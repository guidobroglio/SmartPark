namespace SmartParkPhilipsHue.Models
{
    // Stato della lampadina: contiene le informazioni sullo stato della lampadina
    public class State
    {
        // Stato della lampadina: acceso/spento
        public bool On { get; set; }
        // Luminosità
        public int Bri { get; set; }
        // Colore
        public int Hue { get; set; }
        // Saturazione
        public int Sat { get; set; }
        // Effetto
        public string Effect { get; set; }
        // Coordinate XY
        public float[] XY { get; set; }
        // Temperatura colore
        public int Ct { get; set; }
        // Allarme
        public string Alert { get; set; }
        // Modalità colore
        public string Colormode { get; set; }
        // Modalità raggruppamento
        public bool Reachable { get; set; }
    }
}
