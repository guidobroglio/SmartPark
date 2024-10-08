const id = document.getElementById('id-utente').getAttribute('idutente');


//document.addEventListener('DOMContentLoaded', function () {
//    const posto = document.getElementById('posto-mwbot');
//    const savedText = localStorage.getItem('posto-mwbot-text');

//    if (savedText) {
//        posto.textContent = savedText;
//    } else {
//        posto.textContent = `MWbot non sta ricaricando nessun veicolo.`;
//    }
//});

// Crea connessione hub SignalR 
const connection = new signalR.HubConnectionBuilder()
    .withUrl("https://localhost:7000/notificaHub")
    .build();

connection.on("RicaricaCompletata", (utente, messaggio) => {
    if (utente.toString() == id.toString()) {
        const ricarica = JSON.parse(messaggio);

        const testoModal = document.getElementById('notifica-ricarica');
        testoModal.innerHTML =
            `<p>Ricarica completata per il veicolo "${ricarica.veicolo.targa}".</p>` +
            `<p>Data fine: ${new Date(ricarica.dataFine).toLocaleString()}</p>` +
            `<p>Prezzo ricarica: ${ricarica.prezzoRicarica} €</p>`;

        const modal = new bootstrap.Modal(document.getElementById('notifica-modal'));
        modal.show();
    }
});

// Connetti 
connection.start().then(function () {
    console.log("SignalR Connected.");
}).catch(function (err) {
    return console.error(err.toString());
});
