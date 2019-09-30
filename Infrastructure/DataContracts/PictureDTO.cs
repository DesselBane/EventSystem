using System.Runtime.Serialization;

namespace Infrastructure.DataContracts
{
    [DataContract]
    public class PictureDTO
    {
        #region Properties

        [DataMember]
        public byte[] Picture { get; set; }

        #endregion
    }
}