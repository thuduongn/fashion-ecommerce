$(document).ready(function () {
    $('.product-search').keyup(function () {
        var search = $(this).val();
        console.log(search);
        if (search != '') {
            $.ajax({
                url: '/Home/Search',
                method: 'GET',
                data: {
                    search: search,
                },
                success: function (response) {
                    let data = response;
                    let html = '';
                    for (let i = 0; i < data.length; i++) {
                        let url = `/Products/Details/${data[i].id}`;
                        /*html += `<li>` + data[i].name + `</li>`*/
                        html += `<a href="${url}">` + data[i].name + `</a>`

                        //html += `<a href="${url}" class="search-result">
                        //            <img src="${data[i].img.Split(',')[0]}" alt="${data[i].name}" class="product-image">
                        //            <span class="product-name">${data[i].name}</span>
                        //            <span class="product-price">${data[i].price}</span>
                        //         </a>`;

                        //html += `<div class="dropdown-menu">
                        //            <a href="${url}" class="dropdown-item">
                        //                <img src="#" alt="${data[i].name}" class="product-image">
                        //                <span class="product-name">` + data[i].name + `</span>
                        //                <span class="product-price">` + data[i].name + `</span>
                        //            </a>
                        //         </div>`;

                    }
                    $('#list-product').html(html);
                }
            });
        } else {
            $('#list-product').html('');
        }
    });

    function handleSearch() {
        var search = $('.product-search').val();
        if (search != '') {
            $.ajax({
                url: '/Home/Search',
                method: 'GET',
                data: {
                    search: search,
                },
                success: function (response) {
                    let data = response;
                    let html = '';
                    console.log(data)
                    html += `<div class="container-fluid pt-5">
                                    <div class="text-center mb-4">
                                        <h2 class="section-title px-5"><span class="px-2">Products Result</span></h2>
                                    </div>
                                    <div class="row px-xl-5 pb-3">`;
                    for (let i = 0; i < data.length; i++) {
                        let url = `/Products/Details/${data[i].id}`;
                        /*html += `<li>` + data[i].name + `</li>`*/
                        let imageUrl = data[i].img.split(',')[0];

                        
                        html += `<div class="col-lg-3 col-md-6 col-sm-12 pb-1">
                                        <div class="card product-item border-0 mb-4">
                                            <div class="card-header product-img position-relative overflow-hidden bg-transparent border p-0">
                                                <img class="img-fluid w-100" src="`+ imageUrl +`" alt="">
                                            </div>
                                            <div class="card-body border-left border-right text-center p-0 pt-4 pb-3">
                                                <h6 class="text-truncate mb-3">` + data[i].name + `</h6>
                                                <div class="d-flex justify-content-center">
                                                    <h6>` + data[i].price + `</h6><h6 class="text-muted ml-2"><del>` + data[i].price + `</del></h6>
                                                </div>
                                            </div>
                                            <div class="card-footer d-flex justify-content-between bg-light border">
                                                <a href="`+ url +`" class="btn btn-sm text-dark p-0"><i class="fas fa-eye text-primary mr-1"></i>View Detail</a>
                                                <a product-id="` + data[i].id + `" class="btn btn-sm text-dark p-0 add-to-cart-btn"><i class="fas fa-shopping-cart text-primary mr-1"></i>Add To Cart</a>
                                            </div>
                                        </div>
                                    </div>
                                </div>`
                                
                    };
                    html +=     `</div>
                            </div >`;
                    $('#search-result').html(html);
                }
            })
        }
    }

    $('.product-search').keydown(function (e) {
        if (e.keyCode === 13) { 
            console.log('here')
            e.preventDefault();
            handleSearch();
        }
    });

    $('.input-group-append').click(function () {
        handleSearch();
    });

    $('.rate-review').click(function () {
        let numberStart = $(this).attr('number-star');
        let $inputStar = $('input[name="star"]');
        console.log('vao day');
        $('.rate-review').removeClass('fas');
        /*$('.rate-review').addClass('far');*/

        for (let i = 1; i <= Number(numberStart); i++) {
            console.log(i);

            $("#rate_" + i.toString()).addClass('fas');
        }

        $inputStar.val(numberStart);
    });

    $('.button-minus').click(function () {
        var quantityInput = $('input[name="product-quantity"]');
        var quantity = parseInt(quantityInput.val());

        if (quantity - 1 <= 0) {
            $.notify("Số lượng phải > 0", "warn");
            return false;
        } else {
            quantityInput.val(quantity - 1);
        }
    });

    $('.button-plus').click(function () {
        var quantityInput = $('input[name="product-quantity"]');
        var quantity = parseInt(quantityInput.val());

        quantityInput.val(quantity + 1);
    });

    $('.product-quantity').focusout(function () {
        var inputField = $(this);
        var quantity = Number(inputField.val());
        if (Number(quantity) - 1 <= 0) {
            $.notify("Số lượng phải > 0", "warn");
            return false;
        }
    });

});