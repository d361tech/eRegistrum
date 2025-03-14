namespace Pastoral.ba.Models
{
    public class Osoba
    {
        public int Id { get; set; }
        public string JMBG { get; set; }
        public string PunoIme { get; set; }
        public string Email { get; set; }

        public int ID_Zupe { get; set; }
        public Zupa Zupa { get; set; }
    }
}
