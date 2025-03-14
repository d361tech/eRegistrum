namespace Pastoral.ba.Models
{
    public class Zupa
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int ID_Biskupija { get; set; }
        public Biskupija Biskupija { get; set; }

        public string ID_Zupnik { get; set; }
        public ApplicationUser Zupnik { get; set; }

        public ICollection<ApplicationUser> Kapelan { get; set; }
        public ICollection<Osoba> Osobe { get; set; }
    }
}
