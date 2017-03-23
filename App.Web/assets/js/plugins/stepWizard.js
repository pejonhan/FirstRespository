var StepWizard = function () {

    return {

        initStepWizard: function (isLogin) {
            var form = $(".shopping-cart");
                form.validate({
                    errorPlacement: function errorPlacement(error, element) { element.before(error); },
                    rules: {
                        confirm: {
                            equalTo: "#password"
                        }
                    }
                });
                form.children("div").steps({
                    labels:{
                        next:"下一步",
                        previous:"上一步",
                        finish:"确定兑换",
                    },
                    headerTag: ".header-tags",
                    bodyTag: "section",
                    transitionEffect: "fade",
                    onStepChanging: function (event, currentIndex, newIndex) {
                        //return true;
                        // Allways allow previous action even if the current form is not valid!
                        if (currentIndex > newIndex || newIndex == 3)
                        {
                            return true;
                        }
                        form.validate().settings.ignore = ":disabled,:hidden";
                        var isValid = form.valid();

                        if (isValid) {
                            var ajaxValid = false;
                            //验证手机验证码
                            if (newIndex == 1 && isLogin == 'False') {
                                var mobile = $('input[name="MobileCode"]').val();
                                $.ajax({
                                    async: false,
                                    type: 'POST',
                                    url: '/common/CheckMobileCode',
                                    data: {
                                        mobileCode: mobile
                                    }
                                }).done(function (data) {
                                    ajaxValid = data.success;
                                    if (!data.success) {
                                        sweetAlert(data.message, "", "error");
                                    }
                                });
                                $('#phone2').val(mobile);
                                return ajaxValid;
                            }
                            //验证套餐
                            if (newIndex == 2 || (newIndex == 1 && isLogin == 'True')) {
                                var redeemCode = form.find('input[name="RedeemCode"]').val();

                                if (redeemCode === '' || redeemCode.length < 6) {
                                    sweetAlert("请输入正确的兑换码!", "", "error");
                                    return false;
                                }

                                $.ajax({
                                    async: false,
                                    type: 'POST',
                                    url:'/Goods/CheckRedeemCodeAsync',
                                    data: { redeemCode: redeemCode }
                                }).done(function (data) {
                                    ajaxValid = data.success;
                                    if (data.success) {
                                        $('#package-infomation').html('<h2 class="welcome-title">套餐信息如下</h2>'
                                            + '<p>请点击「下一步」填写收货信息</p>');
                                        $('#package-infomation').append('<p>' + data.message + '</p>');
                                    }
                                    else {
                                        sweetAlert(data.message, "", "error");
                                    }
                                });

                                return ajaxValid;
                            }
                        }
                        
                        return isValid;
                    },
                    onFinishing: function (event, currentIndex) {
                        form.validate().settings.ignore = ":disabled";
                        return form.valid();
                    },
                    onFinished: function (event, currentIndex) {
                        $('#ExchangeForm').ajaxSubmit({
						    beforeSend: function () {
						        blockUI();
						    },
						    success: function (data) {
						        $.unblockUI();
						        if (data.success) {
						            swal({
						                title: "谢谢！",
						                text: "您的订单我们已经收到，我们接下来将会给您安排发货",
						                type: 'success'
						            }, function () {
						                location.href = '/exchange/orders';
						            });
						        }
						        else {
						            sweetAlert(data.message, "", "error");
						        }
						    }
						});
                    }
                });
        }, 

    };
}();        