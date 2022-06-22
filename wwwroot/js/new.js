const myPrivateKey = localStorage.getItem("secret");
const myPublicKey = localStorage.getItem("public");

const crypt = new Crypt();

$("#client-form").submit(function(e) {

    e.preventDefault();

    //Checking if recipient exits
    const recipientUsername = $("#recipient").val();

    if(recipientUsername.match("[^A-Za-z0-9]+")) {
        $(".error-msg").html("User "+recipientUsername+" does not exist!");
        return;
    }

    $(".error-msg").html('<span class="text-muted">Encrypting and sending message... It should not take more than a few seconds.</span>');

    $.get("/Api/GetPublicKey", {username: recipientUsername}, function (recipientPublicKey) {
        
        if(!recipientPublicKey) {
            $(".error-msg").html("User "+recipientUsername+" does not exist!");
            return;
        }

        //Recipient exits, filling form for server with encrypted data
        $(".message-recipient-username").val(recipientUsername);

        const randomKey = CryptoJS.lib.WordArray.random(32).toString();

        const title = CryptoJS.AES.encrypt($("#title").val(), randomKey).toString();
        $(".message-title").val(title);

        if($("#text").val()) {
            const text = CryptoJS.AES.encrypt($("#text").val(), randomKey).toString();
            $(".message-text").val(text);
        }

        const crypt = new Crypt();

        const mySignature = crypt.signature(myPrivateKey, randomKey);

        const senderKey = crypt.encrypt(myPublicKey, randomKey, mySignature);
        $(".message-senderKey").val(senderKey);

        const recipientKey = crypt.encrypt(recipientPublicKey, randomKey, mySignature);
        $(".message-recipientKey").val(recipientKey);

        $(".message-addToContacts").val($("#contactsCheckbox").prop('checked'));

        $("#server-form").submit();
    });
});

if(replyTo != 0) {

    $.get("/Api/GetMessage", {id: replyTo}, function (data) {

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
    
        if(verified) {
            const title = CryptoJS.enc.Utf8.stringify(CryptoJS.AES.decrypt(data.title, decryptedKey.message));
            
            $(".message-replyTo").val(replyTo);
            $("#recipient").val(data.sender.username);
            $("#title").val(`RE: ${title}`.replace("RE: RE:", "RE:"));
        } else {
            window.location.href = "/App/Mailbox";
        }
    });
}