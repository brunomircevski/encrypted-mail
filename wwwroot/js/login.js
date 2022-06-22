$("#client-form").submit(function(e) {
    e.preventDefault();
    
    const password = $('#password').val();

    $('.password-hash').val(CryptoJS.SHA256(password).toString(CryptoJS.enc.Base64));

    localStorage.setItem("passwordTmp", password);

    $("#server-form").submit();
});
