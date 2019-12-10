
namespace Net
{
    public class HSEL_Math
    {
        public HSEL_Math()
        {
            Key = 0;
        }

        public HSEL_Math(byte key)
        {
            Key = key;
        }

        public void Encrypt(ref byte[] pSrc)
        {
            AvalancheEffect_Forward_En(ref pSrc);
            AvalancheEffect_Reverse_En(ref pSrc);
            Crypto(m_Key, ref pSrc);
        }

        public void Decrypt(ref byte[] pSrc)
        {
            Crypto(m_Key, ref pSrc);
            AvalancheEffect_Reverse_De(ref pSrc);
            AvalancheEffect_Forward_De(ref pSrc);
        }

        public byte Key
        {
            get { return m_Key; }
            set { m_Key = value; }
        }

        private void AvalancheEffect_Forward_En(ref byte[] pSrc)
        {
            if (pSrc.Length > 0)
            {
                var len = pSrc.Length - 1;
                for (int i = 0; i < len; ++i)
                {
                    pSrc[i + 1] ^= pSrc[i];
                }
            }
        }

        private void AvalancheEffect_Forward_De(ref byte[] pSrc)
        {
            if (pSrc.Length > 0)
            {
                var index = pSrc.Length - 1;

                while (index > 0)
                {
                    pSrc[index] ^= pSrc[index - 1];
                    --index;
                }
            }
        }

        private void AvalancheEffect_Reverse_En(ref byte[] pSrc)
        {
            if (pSrc.Length > 0)
            {
                var r_index = pSrc.Length - 1;
                while (r_index > 0)
                {
                    pSrc[r_index - 1] ^= pSrc[r_index];
                    --r_index;
                }
            }
        }

        private void AvalancheEffect_Reverse_De(ref byte[] pSrc)
        {
            if (pSrc.Length > 0)
            {
                var max_index = pSrc.Length - 1;
                for (int index = 0; max_index > index; ++index)
                {
                    pSrc[index] ^= pSrc[index + 1];
                }
            }
        }

        private void Crypto(byte key, ref byte[] pSrc)
        {
            if (pSrc.Length > 0)
            {
                bool odd = (pSrc.Length % 2) == 0;

                for (long i = 0; i < pSrc.Length; ++i)
                {
                    bool bUpdate = odd ? ((i % 2) == 0) : ((i % 2) == 1);
                    if (bUpdate)
                    {
                        pSrc[i] ^= key;
                    }
                }
            }
        }

        private byte m_Key;
    }
}
