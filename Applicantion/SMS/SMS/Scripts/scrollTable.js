
function reposHead(e) {
    var h = document.getElementById('headscroll');
    h.scrollLeft = e.scrollLeft;
    var f = document.getElementById('divfrozen');
    f.scrollTop = e.scrollTop;
}
function reposHorizontal(e) {
    var h = document.getElementById('headscroll');
    var c = document.getElementById('contentscroll');
    h.scrollLeft = e.scrollLeft;
    c.scrollLeft = e.scrollLeft;

    var sh = document.getElementById('hscrollpos');
    sh.innerHTML = e.scrollLeft;

    var ch = document.getElementById('contentwidth');
    var ic = document.getElementById('innercontent');
    ch.innerHTML = ic.clientWidth;  //c.scrollWidth;

    var ch2 = document.getElementById('contentheight');
    ch2.innerHTML = ic.clientHeight;  //c.scrollWidth;

    var sp = document.getElementById('scrollwidth');
    sp.innerHTML = e.scrollWidth;
}
function reposVertical(e) {
    var h = document.getElementById('divfrozen');
    var c = document.getElementById('contentscroll');
    h.scrollTop = e.scrollTop;
    c.scrollTop = e.scrollTop;

    var sh = document.getElementById('vscrollpos');
    sh.innerHTML = e.scrollTop;

    var ch = document.getElementById('contentheight');
    ch.innerHTML = c.scrollHeight;

    var sp = document.getElementById('scrollheight');
    sp.innerHTML = e.scrollHeight;

}
