const myPrivateKey = localStorage.getItem("secret");
const myPublicKey = localStorage.getItem("public");

const crypt = new Crypt();

function InsertMessage(id, title, text, unparsedDate, recipient) {

    const humanDate = formatDate(new Date(unparsedDate));

    const template = `<hr>
    <article class="row position-relative">
        <span class="col-7 text-muted">To: ${recipient}</span>
        <span class="col-5 text-end text-muted text-truncate">${humanDate}</span>
        <h5 class="text-truncate">${title}</h5>
        <p class="overflow-hidden text-truncate mb-1 pe-5">${text}</p>
        <a href="/App/Message?id=${id}" class="stretched-link"></a>
    </article>`;

    $(".mail-list").append(template);
}

function GetMessages() {
    $.get("/Api/GetSentMessages", function (data) {
    
        $(".mail-list").html("");
    
        data.forEach(e => {
            const decryptedKey = crypt.decrypt(myPrivateKey, e.senderKey);
    
            const title = CryptoJS.enc.Utf8.stringify(CryptoJS.AES.decrypt(e.title, decryptedKey.message));
            const text = CryptoJS.enc.Utf8.stringify(CryptoJS.AES.decrypt(e.text, decryptedKey.message));
    
            InsertMessage(e.id, sanitizeHhtmlEntities(title), sanitizeHhtmlEntities(text), e.date, e.recipient.username);
        });
    });
}

GetMessages();
