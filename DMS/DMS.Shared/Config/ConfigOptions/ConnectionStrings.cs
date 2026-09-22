using System.ComponentModel.DataAnnotations;

namespace DMS.Shared.Config.Config
{
    public class ConnectionStrings
    {
        private string _dmsDb;
        private string _dmsSharedDb;

        [Required(ErrorMessage = "HeadEndDb is required.")]
        public string DmsDb
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_dmsDb))
                    return _dmsDb;

                return _dmsDb + "Search Path=dms;";
            }
            set => _dmsDb = value;
        }

        [Required(ErrorMessage = "HeadEndTenantDb is required.")]
        public string SharedDb
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_dmsSharedDb))
                    return _dmsSharedDb;

                return _dmsSharedDb + "Search Path=shared;";
            }
            set => _dmsSharedDb = value;
        }
    }
}
