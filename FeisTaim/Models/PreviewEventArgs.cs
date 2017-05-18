using System;

namespace FeisTaim.Models
{
    public class PreviewEventArgs : EventArgs
    {
        public byte[] Data
        {
            get;
            set;
        }
    }
}