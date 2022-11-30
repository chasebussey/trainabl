function FocusLastRow() {
    var rows = document.getElementsByName("movementName");
    var lastRow = rows.item(rows.length - 1);
    console.log("APP: interop called")
    lastRow.focus();
}