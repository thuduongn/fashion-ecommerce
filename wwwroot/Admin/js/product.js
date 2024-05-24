$(document).ready(function () {
    $('.search-products').keyup(function () {
        var name = $(this).val();
        var quantity = $('select[name="quantityFilter"]').val();
        var category = $('select[name="categoryFilter"]').val();
        var status = $('select[name="statusFilter"]').val();
        if (name != '') {
            $.ajax({
                url: '/Admin/Products/FilterProduct',
                method: 'GET',
                data: {
                    quantity: quantity,
                    category: category,
                    status: status,
                    name: name,
                },
                success: function (response) {
                    let data = response;
                    console.log(response);
                    let html = '';
                    for (let i = 0; i < data.length; i++) {
                        let url = `/Admin/Products/Details/${data[i].productId}`;
                        /*html += `<li>` + data[i].name + `</li>`*/
                        html += `<a class="dropdown-menu__item active" href="${url}" tabindex="0" data-value="${data[i].productName}">` + data[i].productName + `</a>`
                    }
                    $('#product-list').html(html);
                },
                error: function () {
                    console.log('Error');
                }
            });
        } else {
            $('#product-list').html('');
        }
    });

    function SearchProducts(quantity, category, status, name) {
        $.ajax({
            url: '/Admin/Products/FilterProduct',
            method: 'GET',
            data: {
                quantity: quantity,
                category: category,
                status: status,
                name: name,
            },

            success: function (response) {
                console.log(response)
                let data = response;
                let html = '';
                for (let i = 0; i < data.length; i++) {
                    html +=
                        `<tr class="table__row">
                                <td class="table__td">
                                    <div class="table__checkbox table__checkbox--all">
                                        <label class="checkbox">
                                            <input type="checkbox" data-checkbox="product"><span class="checkbox__marker">
                                                <span class="checkbox__marker-icon">
                                                    <svg class="icon-icon-checked">
                                                        <use xlink: href="#icon-checked"></use>
                                                    </svg>
                                                </span>
                                            </span>
                                        </label>
                                    </div>
                                </td>
                                <td class="d-none d-lg-table-cell table__td">
                                    <span class="text-grey">`+ data[i].id + `</span>
                                </td>
                                <td class="table__td">`+ data[i].productName + `</td>
                                <td class="table__td">
                                    <span class="text-grey">`+ data[i].productCategories + `</span>
                                </td>
                                <td class="table__td">
                                    <span>`+ data[i].productPrice + `</span>
                                </td>
                                <td class="d-none d-lg-table-cell table__td">
                                    <span class="text-grey">`+ data[i].productDate + `</span>
                                </td>
                                <td class="d-none d-sm-table-cell table__td">`;
                    if (data[i].productDeleted == 1) {
                        html += '<div class="table__status"><span class="table__status-icon color-red"></span> Deleted</div>';
                    } else if (data[i].productStatus == 1) {
                        html += '<div class="table__status"><span class="table__status-icon color-green"></span> Public</div>';
                    } else {
                        html += '<div class="table__status"><span class="table__status-icon color-orange"></span> Private</div>';
                    }
                    html += `</td>
                                <td class="table__td table__actions">
                                    <div class="items-more">
                                        <button class="items-more__button">
                                            <svg class="icon-icon-more">
                                                <use xlink: href="#icon-more"></use>
                                            </svg>
                                        </button>
                                        <div class="dropdown-items dropdown-items--right">
                                            <div class="dropdown-items__container">
                                                <ul class="dropdown-items__list">
                                                    <li class="dropdown-items__item">
                                                        <a class="dropdown-items__link" href="/Admin/Products/Details/${data[i].id}">
                                                            <span class="dropdown-items__link-icon">
                                                                <svg class="icon-icon-view">
                                                                    <use xlink: href="#icon-view"></use>
                                                                </svg>
                                                            </span>Details
                                                        </a>
                                                    </li>
                                                    <li class="dropdown-items__item">
                                                        <a class="dropdown-items__link">
                                                            <span class="dropdown-items__link-icon">
                                                                <svg class="icon-icon-trash">
                                                                    <use xlink: href="#icon-trash"></use>
                                                                </svg>
                                                            </span>Delete
                                                        </a>
                                                    </li>
                                                </ul>
                                            </div>
                                        </div>
                                    </div>
                                </td>
                            </tr>`
                }
                $('#product-search').html(html);
            },
            error: function () {
                console.log('Error');
            }
        });
    }
    $('.search-products').keydown(function (e) {
        if (e.keyCode === 13) {
            e.preventDefault();
            var name = $(this).val();
            var quantity = $('select[name="quantityFilter"]').val();
            var category = $('select[name="categoryFilter"]').val();
            var status = $('select[name="statusFilter"]').val();
            if (name != '') {
                SearchProducts(quantity, category, status, name);
            } else {
                $('#product-search').html('');
            }
        }
    });

    $('select[name="statusFilter"], select[name="quantityFilter"], select[name="categoryFilter"]').on('change', function () {

        var quantity = $('select[name="quantityFilter"]').val();
        var category = $('select[name="categoryFilter"]').val();
        var status = $('select[name="statusFilter"]').val();
        var name = $('.search-products').val()
        console.log(quantity, category, status, name)

        SearchProducts(quantity, category, status, name);
    });

    $('#prev-page').click(function () {
        var quantity = $('select[name="quantityFilter"]').val();
        var category = $('select[name="categoryFilter"]').val();
        var status = $('select[name="statusFilter"]').val();
        var name = $('.search-products').val();
        var currentURL = window.location.href;
        console.log(currentURL);

        // Tạo một đối tượng URLSearchParams từ URL
        var urlParams = new URLSearchParams(currentURL);
        
        // Lấy giá trị của biến 'page'
        var page = urlParams.get('page');
        if (page == null || page == undefined) {
            page = 1;
        }
        console.log(page);
        page = Number(page) == 1 ? 1 : Number(page) - 1;
        console.log(page);
        let url = "http://localhost:5074/Admin/Products/Index?page=" + page + "&quantity=" + quantity + "&category=" + category + "&status=" + status + "&name=" + name;
        console.log(url);
        window.location.href = url;
    });
    $('#next-page').click(function () {
        var quantity = $('select[name="quantityFilter"]').val();
        var category = $('select[name="categoryFilter"]').val();
        var status = $('select[name="statusFilter"]').val();
        var name = $('.search-products').val();
        var currentURL = window.location.href;

        // Tạo một đối tượng URLSearchParams từ URL
        var urlParams = new URLSearchParams(currentURL);

        // Lấy giá trị của biến 'page'
        var page = urlParams.get('page');
        if (page == null || page == undefined) {
            page = 1;
        }
        console.log('page = ' + page);
        page = Number(page) + 1;
       
        let url = "http://localhost:5074/Admin/Products/Index?page=" + page + "&quantity=" + quantity + "&category=" + category + "&status=" + status + "&name=" + name;

        console.log(url);
        window.location.href = url;
    });

    $('#add-order-history').click(function () {
        let status = $('#select-status-order').val();;
        let id = $(this).attr('id-product');
        let url = 'http://localhost:5074/Admin/Orders/AddOrderHistory?id=' + id + '&status=' + status;
        window.location.href = url;
    });

});