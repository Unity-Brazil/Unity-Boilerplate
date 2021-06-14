namespace GleyAllPlatformsSave
{
    public class Encryption
    {
        public static byte[] Encrypt(byte[] bytes)
        {
            for (int index = 0; index < bytes.Length; ++index)
                bytes[index] += (byte)(bytes.Length - 50 + (index + 27) * index);
            return bytes;
        }

        public static byte[] Decrypt(byte[] bytes)
        {
            for (int index = 0; index < bytes.Length; ++index)
                bytes[index] -= (byte)(bytes.Length - 50 + (index + 27) * index);
            return bytes;
        }
    }
}
