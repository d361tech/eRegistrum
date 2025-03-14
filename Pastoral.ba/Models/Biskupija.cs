namespace Pastoral.ba.Models
{
    public class Biskupija
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string ID_GenVik { get; set; }
        public ApplicationUser GenVikar { get; set; }

        public string ID_Ekonom { get; set; }
        public ApplicationUser Ekonom { get; set; }

        public ICollection<Zupa> Zupe { get; set; }
    }
}