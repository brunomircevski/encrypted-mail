const myPrivateKey = localStorage.getItem("secret");
const myPublicKey = localStorage.getItem("public");

const crypt = new Crypt();
let newestId = 0;

function InsertMessage(id, title, text, unparsedDate, sender, verified) {
    if(verified) verified = "";
    else verified = "Sender's signature verification failed!";

    const humanDate = formatDate(new Date(unparsedDate));

    const template = `<hr>
    <article class="row position-relative">
        <span class="col-7 text-muted">From: ${sender}</span>
        <span class="col-5 text-end text-muted text-truncate">${humanDate}</span>
        <span class="col-12 text-danger mb-1">${verified}</span>
        <h5 class="text-truncate">${title}</h5>
        <p class="overflow-hidden text-truncate mb-1 pe-5">${text}</p>
        <a href="/App/Message?id=${id}" class="stretched-link"></a>
    </article>`;

    $(".mail-list").append(template);
}

function GetMessages() {
    $.get("/Api/GetMyMessages", function (data) {
    
        $(".mail-list").html("");
        newestId = data[0].id;
    
        data.forEach(e => {
            const decryptedKey = crypt.decrypt(myPrivateKey, e.recipientKey);
            
            const verified = crypt.verify(
                e.sender.publicKey,
                decryptedKey.signature,
                decryptedKey.message,
            );
    
            const title = CryptoJS.enc.Utf8.stringify(CryptoJS.AES.decrypt(e.title, decryptedKey.message));
            const text = CryptoJS.enc.Utf8.stringify(CryptoJS.AES.decrypt(e.text, decryptedKey.message));
    
            InsertMessage(e.id, sanitizeHhtmlEntities(title), sanitizeHhtmlEntities(text), e.date, e.sender.username, verified);
        });
    });
}

GetMessages();

setInterval(function () {
    $.get("/Api/GetNewestMessageId", function (data) {
        if(data != newestId && data != 0) GetMessages();
    });
}, 5000);