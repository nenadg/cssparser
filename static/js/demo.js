
$(document).ready(function () {
    
    $('#submit').on('click', (function () {

        // 'CssProperties' should represent IEnumerable<CssProperty>
        // that our post action needs 
        var CssProperties = [];
        var serialized = $('#super-form').serializeArray();
        
        serialized.forEach(function (e) {

            // here we recreate class CssProperty
            var CssProperty = {
                name: e.name.split('.')[1],
                type: e.name.split('.')[0],
                values: []
            };

            // and 'values' is the same thing as IEnumerable<CssValue>
            CssProperty.values.push({
                name: e.name.split('.')[2],
                value: e.value
            })
            
            CssProperties.push(CssProperty);
        });

        // if posting arrays to enumerable types, one must set
        // contentType to 'application/json' otherwise our
        // controller action parameter will be null
        $.ajax({
            type: "POST",
            url: "/api/cssparse/post",
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify(CssProperties)
        }).done(function () {
            
            // since ajax can't download, you allegedly had to use a 'iframe' hack
            // so you could create invisible iframe which points to download url
            // (or get one from response), something like this:
            // $("body").append("<iframe src='/api/cssparse/download' id='__superduperdownloader' style='display: none;'></iframe>");

            // but I came with better solution:
            // I created separate controller that only responds 
            // with appropriate content headers so you can just
            // point your window.location to it's url and you get
            // your file:
            window.location.href = '/api/cssparse/download';
        }).fail(function (xhr) {

            // display some error message
            console.log(xhr.responseJSON.Message);
        });
    }));
});