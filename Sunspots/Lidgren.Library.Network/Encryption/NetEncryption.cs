using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace Lidgren.Library.Network
{
	/// <summary>
	/// Helper class for encryption
	/// </summary>
	public sealed class NetEncryption
	{
		private byte[] m_symEncKeyBytes;
		private RSACryptoServiceProvider m_rsa;
		private XTEA m_xtea;
		private int[] m_symmetricKey;

		internal byte[] SymmetricEncryptionKeyBytes { get { return m_symEncKeyBytes; } }

		/// <summary>
		/// Generate an RSA keypair, divided into public and private parts
		/// </summary>
		public static void GenerateRandomKeyPair(out byte[] publicKey, out byte[] privateKey)
		{
			RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
			RSAParameters prm = rsa.ExportParameters(true);

			List<byte> pubKey = new List<byte>(131);
			pubKey.AddRange(prm.Exponent);
			pubKey.AddRange(prm.Modulus);
			
			List<byte> privKey = new List<byte>(448);
			privKey.AddRange(prm.D); // 128
			privKey.AddRange(prm.DP); // 64
			privKey.AddRange(prm.DQ); // 64
			privKey.AddRange(prm.InverseQ); // 64
			privKey.AddRange(prm.P); // 64
			privKey.AddRange(prm.Q); // 64

			publicKey = pubKey.ToArray();
			privateKey = privKey.ToArray();
		}

		public NetEncryption()
		{
		}

		internal void SetSymmetricKey(byte[] xteakey)
		{
			if (xteakey == null || xteakey.Length != 16)
				throw new NetException("Bad symmetric key length (" + xteakey.Length + ") must be 16!");
			m_symEncKeyBytes = xteakey;
			m_symmetricKey = new int[4];
			m_symmetricKey[0] = BitConverter.ToInt32(xteakey, 0);
			m_symmetricKey[1] = BitConverter.ToInt32(xteakey, 4);
			m_symmetricKey[2] = BitConverter.ToInt32(xteakey, 8);
			m_symmetricKey[3] = BitConverter.ToInt32(xteakey, 12);
			m_xtea = new XTEA(xteakey, 32);
		}

		/// <summary>
		/// for clients; pass null as privateKey
		/// </summary>
		internal void SetRSAKey(byte[] publicKey, byte[] privateKey)
		{
			m_rsa = new RSACryptoServiceProvider();

			RSAParameters prm = new RSAParameters();
			prm.Exponent = Extract(publicKey, 0, 3);
			prm.Modulus = Extract(publicKey, 3, 128);

			if (privateKey != null)
			{
				int ptr = 0;
				prm.D = Extract(privateKey, ptr, 128); ptr += 128;
				prm.DP = Extract(privateKey, ptr, 64); ptr += 64;
				prm.DQ = Extract(privateKey, ptr, 64); ptr += 64;
				prm.InverseQ = Extract(privateKey, ptr, 64); ptr += 64;
				prm.P = Extract(privateKey, ptr, 64); ptr += 64;
				prm.Q = Extract(privateKey, ptr, 64); ptr += 64;
			}

			m_rsa.ImportParameters(prm);

			// also generate random symmetric key
			byte[] newKey = new byte[16];
			NetRandom.Default.NextBytes(newKey);
			SetSymmetricKey(newKey);
		}

		private static byte[] Extract(byte[] buf, int start, int len)
		{
			byte[] retval = new byte[len];
			Array.Copy(buf, start, retval, 0, len);
			return retval;
		}

		/// <summary>
		/// Encrypt data using a public RSA key
		/// </summary>
		internal byte[] EncryptRSA(byte[] plainData)
		{
			return m_rsa.Encrypt(plainData, false);
		}

		/// <summary>
		/// Decrypt data using the public and private RSA key
		/// </summary>
		internal byte[] DecryptRSA(byte[] encryptedData)
		{
			try
			{
				return m_rsa.Decrypt(encryptedData, false);
			}
			catch (Exception ex)
			{
				if (NetBase.CurrentContext != null && NetBase.CurrentContext.Log != null)
					NetBase.CurrentContext.Log.Warning("Failed to Decrypt RSA: " + ex);
				return null;
			}
		}

		/// <summary>
		/// Append a CRC checksum and encrypt data in place using XTEA
		/// </summary>
		internal void EncryptSymmetric(NetBuffer buffer)
		{
			//string plain = Convert.ToBase64String(buffer.Data, 0, buffer.LengthBytes);
			int bufferLen = buffer.LengthBytes;

			// calculate number of pad bits
			int dataBits = buffer.LengthBits;
			int bitsNeeded = dataBits + 24; // 24 extra for crc and num-pad-bits
			int totalBits = bitsNeeded + (64 - (bitsNeeded % 64));
			int padBits = totalBits - bitsNeeded;

			// write to ensure zeroes in buffer (crc and num-pad-bits space)
			buffer.Write((uint)0, 24);

			if (padBits > 0)
				buffer.Write((ulong)0, padBits);
			int writePadBitsPosition = buffer.LengthBits - 8;

			// write crc
			ushort crc = Checksum.Adler16(buffer.Data, 0, buffer.LengthBytes);
			buffer.ResetWritePointer(dataBits);
			buffer.Write(crc);

			// write num-pad-bits in LAST byte
			buffer.ResetWritePointer(writePadBitsPosition);
			buffer.Write((byte)padBits);

			// encrypt in place
			int ptr = 0;
			bufferLen = buffer.LengthBytes;
			while (ptr < bufferLen)
			{
				m_xtea.EncryptBlock(buffer.Data, ptr, buffer.Data, ptr);
				ptr += 8;
			}

			return;
		}
		
		/// <summary> 
		/// Decrypt using XTEA algo and verify CRC
		/// </summary>
		/// <returns>true for success, false for failure</returns>
		internal bool DecryptSymmetric(NetBuffer buffer)
		{
			int bufLen = buffer.LengthBytes;

			if (bufLen % 8 != 0)
			{
				if (NetBase.CurrentContext != null && NetBase.CurrentContext.Log != null)
					NetBase.CurrentContext.Log.Info("Bad buffer size in DecryptSymmetricInPlace()");
				return false;
			}

			//NetBase.CurrentContext.Log.Debug.Debug("Decrypting using key: " + Convert.ToBase64String(m_xtea.Key));

			// decrypt
			for (int i = 0; i < bufLen; i += 8)
				m_xtea.DecryptBlock(buffer.Data, i, buffer.Data, i);

			int numPadBits = buffer.Data[bufLen - 1];
			buffer.Data[bufLen - 1] = 0; // zap for correct crc calculation
			int dataBits = (bufLen * 8) - (24 + numPadBits); // include pad and crc

			buffer.ResetReadPointer(dataBits);
			ushort statedCrc = buffer.ReadUInt16();

			// zap crc to be able to compare
			buffer.ResetWritePointer(dataBits);
			buffer.Write((ushort)0);

			ushort dataCrc = Checksum.Adler16(buffer.Data, 0, bufLen);

			//NetBase.CurrentContext.Log.Debug("Plain (len " + bufLen + "): " + Convert.ToBase64String(buffer.Data, 0, bufLen) + " Stated CRC: " + statedCrc + " Calc: " + realCrc);
			if (statedCrc != dataCrc)
			{
				if (NetBase.CurrentContext != null && NetBase.CurrentContext.Log != null)
					NetBase.CurrentContext.Log.Warning("CRC failure; expected " + dataCrc + " found " + statedCrc + " dropping packet!");
				return false;
			}

			// clean up
			buffer.LengthBits = dataBits;
			buffer.ResetReadPointer();

			return true;
		}

		public static bool SimpleUnitTest()
		{
			// encryption test
			NetEncryption enc = new NetEncryption();
			byte[] key = new byte[16];
			NetRandom.Default.NextBytes(key);
			enc.SetSymmetricKey(key);

			byte[] data = Encoding.ASCII.GetBytes("Michael"); // 7 bytes
			byte[] correct = new byte[data.Length];
			data.CopyTo(correct, 0);

			NetBuffer buf = new NetBuffer(data);

			enc.EncryptSymmetric(buf);

			bool ok = enc.DecryptSymmetric(buf);
			if (!ok)
				return false;

			// compare
			if (buf.LengthBytes != correct.Length)
				return false;

			for (int i = 0; i < correct.Length; i++)
				if (buf.Data[i] != correct[i])
					return false;

			return true;
		}
	}
}
