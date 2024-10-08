namespace SmartParkPhilipsHue.Core
{
    // Lampadina Philips Hue
    public class Light
    {
        // Identificativo della lampadina
        public string Id { get; set; }
        // Nome della lampadina
        public string Name { get; set; }
        // Tipo della lampadina
        public Models.State State { get; set; }
    }
}