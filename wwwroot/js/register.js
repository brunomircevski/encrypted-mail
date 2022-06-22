$("#client-form").submit(function(e) {
    e.preventDefault();

    const username = $('.username').val();
    const password = $('#password').val();
    const password2 = $('#password2').val();

    //Validate user input
    if(!(username && password && password2)) {
        $('.error-msg').html("Fill the form first.");
        return;
    }
    
    if(username.length < 5 || username.length > 32) {
        $('.error-msg').html("Username must be 5-32 characters long.");
        return;
    }

    if(username.match("[^A-Za-z0-9]+")) { //Only letters and numbers
        $('.error-msg').html("Username can contain only letters and numbers.");
        return;
    }

    if(password.length <=5 || password.length > 32) {
        $('.error-msg').html("Password must be 6-32 characters long.");
        return;
    }

    const passwordReqex = "\S*(\S*([a-zA-Z]\S*[0-9])|([0-9]\S*[a-zA-Z]))\S*"; //At least one letter and one number and no spaces

    if(!password.match(passwordReqex)) {
        $('.error-msg').html("Password must contain at least one letter, one number and no spaces.");
        return;
    }

    if(password != password2) {
        $('.error-msg').html("Passwords are not the same!");
        return;
    }

    //Hash password
    $('.password-hash').val(CryptoJS.SHA256(password).toString(CryptoJS.enc.Base64));

    //Generate key pair
    const rsa = new RSA({
        keySize: 2048
    });
 
    rsa.generateKeyPair(function(keyPair) {
        const publicKey = keyPair.publicKey;
        const privateKey = keyPair.privateKey;

        const encryptedSecret = CryptoJS.AES.encrypt(privateKey, password).toString();

        $(".encryptedPrivateKey").val(encryptedSecret);
        $(".publicKey").val(publicKey); 

        $("#server-form").submit();
    });
    
});