const myPrivateKey = localStorage.getItem("secret");
const myPublicKey = localStorage.getItem("public");

const crypt = new Crypt();

function InsertMessage(title, text, unparsedDate, sender, recipient, verified, replyToId) {
    if(verified) verified = "";
    else verified = "Sender's signature verification failed!";

    if(replyToId) replyToId = `<a href="/App/Message?id=${replyToId}">View previous message</a>`;
    else replyToId = ""; 

    const humanDate = formatDate(new Date(unparsedDate));

    const template = `
    <span class="col-7 text-muted">
    From: <span class="text-light">${sender}</span><br>
    To: ${recipient}
    </span>
    <span class="col-5 text-end text-muted text-truncate">${humanDate}</span>
    <span class="col-12 text-danger mb-1">${verified}</span>
    <span class="col-12 text-light mb-1">${replyToId}</span>
    <h4 class="mb-3 mt-1">${title}</h4>
    <p class="text-justify text-break">${text}</p>`;

    $(".message-box").append(template);
}

$.get("/Api/GetMessage", {id: messageId}, function (data) {

    let encryptedKey;
    let publicKey = myPublicKey;

    if(data.senderKey != null) {
        encryptedKey = data.senderKey;
    }
    else {
        encryptedKey = data.recipientKey;
        publicKey = data.sender.publicKey;
    }

    const decryptedKey = crypt.decrypt(myPrivateKey, encryptedKey);
    
    const verified = crypt.verify(
        publicKey,
        decryptedKey.signature,
        decryptedKey.message,
    );

    const title = CryptoJS.enc.Utf8.stringify(CryptoJS.AES.decrypt(data.title, decryptedKey.message));
    const text = CryptoJS.enc.Utf8.stringify(CryptoJS.AES.decrypt(data.text, decryptedKey.message));

    InsertMessage(sanitizeHhtmlEntities(title), sanitizeHhtmlEntities(text, true), data.date, data.sender.username, data.recipient.username, verified, data.replyToId);
});

document.getElementById('delete-modal').addEventListener('shown.bs.modal', function () {
    document.getElementById('delete-btn').focus();
});