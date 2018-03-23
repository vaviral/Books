function SearchBook() {
    var bookName = document.getElementById("SearchBookBox").value;
    if (bookName == '') {
        alert("Please enter name of the book");
    }
    else {
        var url = "/Books/SearchBooks?BookName=" + bookName;
        window.location = url;
    }
}
function ViewBook(ISBN)
{
    var Url = "/Books/ViewBook?ISBN=" + ISBN;
    window.location = Url;
}
//function AjaxCall(Url, )
function AddToFav(ISBN, BookName)
{
    //var ISBN = document.getElementById("_ISBN").value;
    //var BookName = document.getElementById("BookName").value;
    var Url = "/Books/AddToFavourites";
    $.ajax
        ({
            url: Url,
            dataType: "json",
            type: "GET",
            data: { 'ISBN': ISBN, 'BookName': BookName },
            success: function (response)
            {
                if (response.Status === 1)
                {
                    console.log(response);
                    alert(BookName + " is added to your favourites");
                    $("#AddToFavButton").hide();
                    $("#DeleteFromFavButton").show();
                }
                else if (response.Status === 2)
                {
                    alert(BookName + " is already there in your favourites");
                }
                else
                {
                    alert("Failed to add " + BookName + " to your favourites");
                }
            }
        });
}
function DeleteFromFav(ISBN, BookName)
{
    //var ISBN = document.getElementById("_ISBN").value;
    //var BookName = document.getElementById("BookName").value;
    var Url = "/Books/DeleteFromFavourites";
    //window.location = "/Books/DeleteFromFavourites?ISBN=" + ISBN + "&BookName=" + BookName;

    $.ajax
        ({
            url: Url,
            dataType: "json",
            type: "GET",
            data: { 'ISBN': ISBN, 'BookName': BookName },
            success: function (response) {
                console.log(response);
                if (response.Status === 1) {
                    alert("'" + BookName + "' is removed from your favourites");
                    $("#DeleteFromFavButton").hide();
                    $("#AddToFavButton").show();
                }
                else if (response.Status === 2) {
                    alert("'" + BookName + " is was not there in your favourites");
                }
                else {
                    alert("Failed to remove '" + BookName + "' from your favourites");
                }
            },
            error: function (data) {
                alert(data + "there was some error");
            }
        });
}
