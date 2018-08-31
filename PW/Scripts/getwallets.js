var list = $("#users-list");
$("#To-field").on("input", function () {
    if ($(this).val().length > 0) {
        var obj = { val: $(this).val() };
        $.ajax({
            url: "/Wallet/GetWallets",
            type: "POST",
            data: obj,
            success: function (data) {
                var result = JSON.parse(data);
                list.empty();
                $(result).each(function (index, obj) {
                    list.append($("<option value='" + obj + "'>"))
                });
            }
        });
    }
});