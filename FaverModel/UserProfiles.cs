using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;

namespace Faver2.FaverModel
{
    public class UserProfiles
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string UserName { get; set; }
        public byte[] Gambar { get; set; }
        public string MyKadNumber { get; set; }

        public ICollection<ClockIn>? ClockIns { get; set; }
    }

    public class ClockIn
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ClockInId { get; set; }
        public DateTime ClockInTime { get; set; }

        [ForeignKey(nameof(UserProfiles))]
        public int UserProfilePenggunaId { get; set; }
        public UserProfiles UserProfiles { get; set; }

    }

}
