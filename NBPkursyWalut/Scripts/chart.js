$(document).ready(function () {
    //Load chart if document 'Currency/:currencyCode has been loaded'.
    var url = document.URL;
    var id = url.substring(url.lastIndexOf('/') + 1);


    var chart = c3.generate({
        data: {

            x:'x',
            url:'/Home/GetAverages/' + id,
            mimeType: 'json',
            names: {
                data: id
               
            }



        },
        axis: {
            x: {
                type: 'timeseries',
                tick: {
                    format: '%Y-%m-%d'
                }
               
            }
        }
    });






});


//Reload chart if dates range has been changed. It sends date FROM and date TO with query string to server controller
function loadChart() {

   
    var url = document.URL;
    var id = url.substring(url.lastIndexOf('/') + 1);
   
    var from = document.getElementById('min');
    var to = document.getElementById('max');
   


    var fromValue = from.options[from.selectedIndex].text;
    var toValue = to.options[to.selectedIndex].text;

 

    var chart = c3.generate({
        data: {

            x: 'x',
            url: '/Home/GetAverages/' + id + '?from=' + fromValue + '&to=' + toValue + '',
            mimeType: 'json',
            names: {
                data: id

            }



        },
        axis: {
            x: {
                type: 'timeseries',
                tick: {
                    format: '%Y-%m-%d'
                }

            }
        }
    });


   

}
   









