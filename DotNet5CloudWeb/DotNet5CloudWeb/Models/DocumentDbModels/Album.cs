namespace DotNet5CloudWeb.Models.DocumentDbModels {
    public class Album {
        public string ArtistName { get; set; }

        public string Name { get; set; }
        public int Year { get; set; }
        public string Label { get; set; }
        public int CopiesSold { get; set; }

//        public Member[] Members { get; set; }
    }

    //public class Member {
    //    public string FirstName { get; set; }
    //    public string LastName { get; set; }
    //    public Instrument Instrument { get; set; }
    //}

    //public enum Instrument {
    //    Vocals,
    //    Guitar,
    //    Bass,
    //    Drum,
    //    Keyboards
    //}
}
