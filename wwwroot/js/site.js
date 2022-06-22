$(document).ready(function() {
    $('.navbar-collapse.collapse').find('[href="'+window.location.pathname+'"]').addClass("active");
});

function isToday(date) {
    const today = new Date();
    return date.getDate() == today.getDate() && date.getMonth() == today.getMonth() && date.getFullYear() == today.getFullYear();
}

function padTo2Digits(num) {
    return String(num).padStart(2, '0');
}

function formatDate(d) { 
    let dateStr = `${padTo2Digits(d.getHours())}:${padTo2Digits(d.getMinutes())}`;

    if(!isToday(d)) dateStr = `${padTo2Digits(d.getDate())}-${padTo2Digits(d.getMonth()+1)}-${d.getFullYear()} ` + dateStr;

    return dateStr;
}

function sanitizeHhtmlEntities(str, allowBasic = false) {
    let output = String(str).replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/"/g, '&quot;');
    if(allowBasic) output = output.replace(/&lt;br&gt;/g, '<br>').replace(/&lt;b&gt;/g, '<b>').replace(/&lt;\/b&gt;/g, '</b>');
    return output;
}