$(document).ready(function () {
    function updateCartCount() {
        $.ajax({
            url: '/Carts/GetCartCount',
            method: 'GET',
            success: function (response) {
                $('#cartCount').text(response.cartCount);
            },
            error: function () {
                alert('Có lỗi xảy ra khi thêm vào giỏ hàng.');
            }
        });
    }
    updateCartCount();
    updateCartSummary();
    $('.add-to-cart-btn').click(function () {
        var productId = $(this).attr('product-id');
        var detailQuantity = $('input[name="product-quantity"]').val();
        console.log(detailQuantity)
        $.ajax({
            url: '/Carts/AddToCart/' + productId,
            method: 'POST',
            data: {
                detailQuantity: detailQuantity
            },
            success: function (response) {
                if (response.success) {
                    $.notify("Added to Cart!", "success");
                    updateCartSummary();
                    updateCartCount();
                } else {
                    $.notify(response.content, "warn");
                    setTimeout(function () {
                        window.location.href = 'http://localhost:5074/home/login';
                    }, 2000);
                }
            },
            error: function () {
                alert('Error when adding product to Cart');
            }
        });
    });
    function updateCartSummary() {
        $.ajax({
            url: '/Carts/GetCartSummary',
            method: 'GET',
            success: function (response) {
                $('.sub-total').text('$' + response.subTotal);
                $('.total-summary').text('$' + response.totalSummary);
            },
            error: function () {
                alert('Có lỗi xảy ra khi tinh cart summary.');
            }
        });
    }
    
    $('.quantity-product').focusout(function () {
        var productId = $(this).attr('product-id');
        var inputField = $(this);
        var quantity = Number(inputField.val());
        if (Number(quantity) - 1 <= 0) {
            $.notify("Số lượng phải > 0", "warn");
            return false;
        }
        let price = Number(inputField.attr('price-product'));        

        $.ajax({
            url: '/Carts/AddToCart',
            data: {
                id: productId,
                type: 'input',
                quantity: quantity
            },
            method: 'POST',

            success: function (response) {
                if (response.success) {
                    $.notify("thành công!", "success");
                    updateCartCount();
                    updateCartSummary();
                    let newPrice = quantity * price;
                    var totalPriceCell = inputField.closest('tr').find('.total-price');
                    totalPriceCell.text('$' + newPrice);
                } else {
                    $.notify(response.content, "warn");
                }
            },
            error: function () {
                alert('Có lỗi xảy ra khi thêm vào giỏ hàng.');
            }
        });
    });

    $('.btn-plus').click(function () {
        var clickedButton = $(this);
        var productId = $(this).attr('product-id');
        let quantity = $(this).closest('td').find('.quantity-product').val();
        let price = $(this).attr('price-product');
        $.ajax({
            url: '/Carts/AddToCart/',
            data: {
                id: productId,
                type: 'plus'
            },
            method: 'POST',
            success: function (response) {
                if (response.success) {
                    $.notify("Tăng thành công!", "success");
                    clickedButton.closest('td').find('.quantity-product').val(Number(quantity) + 1);
                    let newPrice = (Number(quantity) + 1) * price;
                    var totalPriceCell = clickedButton.closest('tr').find('.total-price');
                    totalPriceCell.text('$' + newPrice);
                    updateCartCount();
                    updateCartSummary();
                } else {
                    $.notify(response.content, "warn");
                }
            },
            error: function () {
                alert('Có lỗi xảy ra khi thêm vào giỏ hàng.');
            }
        });
    });
    $('.btn-minus').click(function () {
        var clickedButton = $(this);
        var productId = $(this).attr('product-id');
        let quantity = $(this).closest('td').find('.quantity-product').val();
        if (Number(quantity) - 1 <= 0) {
            $.notify("Số lượng phải > 0", "warn");
            return false;
        }
        let price = $(this).attr('price-product');

        $.ajax({
            url: '/Carts/AddToCart/' + productId,
            data: {
                id: productId,
                type: 'minus'
            },
            method: 'POST',
            success: function (response) {
                if (response.success) {
                    $.notify("Giảm thành công!", "success");
                    clickedButton.closest('td').find('.quantity-product').val(Number(quantity) - 1);
                    let newPrice = (Number(quantity) - 1) * price;
                    var totalPriceCell = clickedButton.closest('tr').find('.total-price');
                    totalPriceCell.text('$' + newPrice);
                    updateCartCount();
                    updateCartSummary();
                } else {
                    $.notify(response.content, "warn");
                }
            },
            error: function () {
                alert('Có lỗi xảy ra khi thêm vào giỏ hàng.');
            }
        });
    });

    $('.remove-button').click(function () {
        var clickedButton = $(this);
        var productId = $(this).attr('product-id');
        var ques = confirm("Are you sure you want to delete?")
        if (ques) {
            $.ajax({
                url: '/Carts/RemoveFromCart/' + productId,
                method: 'POST',
                success: function (response) {
                    if (response.success) {
                        clickedButton.closest('tr').remove();
                        $.notify("Xoa thành công!", "success");
                        updateCartCount();
                        updateCartSummary();
                    } else {
                        $.notify(response.content, "warn");
                    }
                },
                error: function () {
                    alert('Có lỗi xảy ra khi xoa giỏ hàng.');
                }
            });
        }
    });

    $("#place-order").click(function () {
        let note = $("#note-order").val();
        console.log(note);
        console.log('vào day');
        $.ajax({
            url: '/Carts/PlaceOrder/',
            data: {
                note: note,
            },
            method: 'POST',
            success: function (response) {
                if (response.success) {
                    $.notify("Thanh toán thành công", "success");
                    location.reload();
                } else {
                    $.notify(response.content, "warn");
                }
            },
            error: function () {
                alert('Có lỗi xảy ra khi thanh toán.');
            }
        });

    })
});