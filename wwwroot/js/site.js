// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Toastr ayarları
toastr.options = {
    "closeButton": true,
    "debug": false,
    "newestOnTop": true,
    "progressBar": true,
    "positionClass": "toast-top-right",
    "preventDuplicates": false,
    "onclick": null,
    "showDuration": "300",
    "hideDuration": "1000",
    "timeOut": "5000",
    "extendedTimeOut": "1000",
    "showEasing": "swing",
    "hideEasing": "linear",
    "showMethod": "fadeIn",
    "hideMethod": "fadeOut"
};

// AJAX hata yakalama
$(document).ajaxError(function(event, jqXHR, settings, error) {
    let errorMessage = "Bir hata oluştu. Lütfen daha sonra tekrar deneyin.";
    
    if (jqXHR.responseJSON && jqXHR.responseJSON.message) {
        errorMessage = jqXHR.responseJSON.message;
    }
    
    toastr.error(errorMessage);
});

// Form submit hata yakalama
$(document).on('submit', 'form', function(e) {
    if (!$(this).hasClass('no-validate')) {
        e.preventDefault();
        const form = $(this);
        
        $.ajax({
            url: form.attr('action'),
            type: form.attr('method'),
            data: form.serialize(),
            success: function(response) {
                if (response.success) {
                    toastr.success(response.message || 'İşlem başarıyla tamamlandı');
                    if (response.redirect) {
                        setTimeout(function() {
                            window.location.href = response.redirect;
                        }, 1000);
                    }
                } else {
                    toastr.error(response.message || 'Bir hata oluştu');
                }
            },
            error: function() {
                toastr.error('Bir hata oluştu. Lütfen daha sonra tekrar deneyin.');
            }
        });
    }
});

// Ev listesi filtreleme
$(document).ready(function() {
    // Tarih seçicilerin minimum tarihini bugün olarak ayarla
    var today = new Date().toISOString().split('T')[0];
    $('#girisTarihi').attr('min', today);
    $('#cikisTarihi').attr('min', today);

    // Giriş tarihi değiştiğinde çıkış tarihinin minimum değerini güncelle
    $('#girisTarihi').on('change', function() {
        var girisTarihi = $(this).val();
        if (girisTarihi) {
            $('#cikisTarihi').attr('min', girisTarihi);
            if ($('#cikisTarihi').val() && $('#cikisTarihi').val() < girisTarihi) {
                $('#cikisTarihi').val(girisTarihi);
            }
        }
    });

    // Çıkış tarihi değiştiğinde giriş tarihinin maximum değerini güncelle
    $('#cikisTarihi').on('change', function() {
        var cikisTarihi = $(this).val();
        if (cikisTarihi) {
            $('#girisTarihi').attr('max', cikisTarihi);
            if ($('#girisTarihi').val() && $('#girisTarihi').val() > cikisTarihi) {
                $('#girisTarihi').val(cikisTarihi);
            }
        }
    });

    // Filtre formu submit edildiğinde
    $('#filterForm').on('submit', function(e) {
        e.preventDefault();
        var formData = $(this).serialize();
        
        $.ajax({
            url: $(this).attr('action'),
            type: 'GET',
            data: formData,
            headers: {
                'X-Requested-With': 'XMLHttpRequest'
            },
            success: function(response) {
                $('#evListesi').html(response);
            },
            error: function(xhr, status, error) {
                console.error('Hata:', error);
                alert('Filtreleme sırasında bir hata oluştu: ' + error);
            }
        });
    });
});
