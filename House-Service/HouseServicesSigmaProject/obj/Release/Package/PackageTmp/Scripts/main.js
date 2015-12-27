$(function () {
    $("img").mouseover(function () {
        $(this).animate({ height: '+=20', width: '+=20' });
    });
    $("img").mouseout(function () {
        $(this).animate({ height: '-=20', width: '-=20' });
    });
});






function fromApi()
{
    $.post('~/Views/Home/Index.cshtml', function (data) {        
        $('.result').html(data);
        alert('Загрузка завершена.');
    });
}

function beautifulMenu()
{
    $('.addMenu').mouseover(function()
    {
        
    })
}


$(function () {  
    $('#SubRegionId').change(function () {
        var id = $(this).val();
        $('#StreetId').prop('disabled', 'true');
        $.ajax({
            type: 'GET',
            data: { SubRegionId: id },
            url: $('#street-to-load').val(),
            error:function () { 
                
            },
            success: function (data) {
                $('#StreetId').empty();
                var optionEmpty = $("<option>");
                $('#StreetId').append(optionEmpty);
                $('#StreetId').append(data);
                $('#StreetId').removeAttr('disabled');
            }
        });
    });

    $('#StreetId').change(function () {
        var id = $(this).val();
        $('#HouseId').prop('disabled', 'true');
        $.ajax({
            type: 'GET',
            data: { StreetId: id },
            url: $('#houses-to-load').val(),
            error: function () {
                
            },
            success: function (data) {

                $('#HouseId').empty();
                var optionEmpty = $("<option>");
                $('#HouseId').append(optionEmpty);
                $('#HouseId').append(data);
                $('#HouseId').removeAttr('disabled');
            }
        });
    });
    
    $('#HouseId').change(function () {
        var id = $(this).val();
        $('#PorchId').prop('disabled', 'true');
        $.ajax({
            type: 'GET',
            data: { HouseId: id },
            url: $('#porches-to-load').val(),
            error: function () {
                
            },
            success: function (data) {

                $('#PorchId').empty();
                var optionEmpty = $("<option>");
                $('#PorchId').append(optionEmpty);
                $('#PorchId').append(data);
                $('#PorchId').removeAttr('disabled');
            }

        });
    });
});

$(function () {
    $('#textArea').mouseover(function () {
        $(this).empty();
    })
});

$(function () {

    $('#CheckAll').click(function () {
        
        $('.MyCheck').attr('checked', true);   
    })
   
});


$(function () {
    
    $('#DisableAll').click(function () {
        location.reload()
        $('.MyCheck').removeAttr('checked', true);
    })

});
