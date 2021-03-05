var App = function () {
    var e, t, n, r, i, s, o;
    var u = function () {
        e = $("#page-container");
        t = $("#page-content");
        n = $("header");
        r = $("#page-content + footer");
        i = $("#sidebar");
        s = $("#sidebar-alt");
        o = $(".sidebar-scroll");
        h("init");
        l();
        m();
        g();
        b();
        v();
        $(window).resize(function () {
            v()
        });
        $(window).bind("orientationchange", v);
        var u = $("#year-copy")
          , a = new Date;
        if (a.getFullYear() === 2014) {
            u.html("2014")
        } else {
            u.html("2014-" + a.getFullYear().toString().substr(2, 2))
        }
        y();
        $('[data-toggle="tabs"] a, .enable-tabs a').click(function (e) {
            e.preventDefault();
            $(this).tab("show")
        });
        $('[data-toggle="tooltip"], .enable-tooltip').tooltip({
            container: "body",
            animation: false
        });
        $('[data-toggle="popover"], .enable-popover').popover({
            container: "body",
            animation: true
        });
        $('[data-toggle="lightbox-image"]').magnificPopup({
            type: "image",
            image: {
                titleSrc: "title"
            }
        });
        $('[data-toggle="lightbox-gallery"]').magnificPopup({
            delegate: "a.gallery-link",
            type: "image",
            gallery: {
                enabled: true,
                navigateByImgClick: true,
                arrowMarkup: '<button type="button" class="mfp-arrow mfp-arrow-%dir%" title="%title%"></button>',
                tPrev: "Previous",
                tNext: "Next",
                tCounter: '<span class="mfp-counter">%curr% of %total%</span>'
            },
            image: {
                titleSrc: "title"
            }
        });
        $(".textarea-editor").wysihtml5();
        $(".select-chosen").chosen({
            width: "100%"
        });
        $(".select-select2").select2();
        $(".input-slider").slider();
        $(".input-tags").tagsInput({
            width: "auto",
            height: "auto"
        });
        //$(".input-datepicker, .input-daterange").datepicker({
        //    weekStart: 1
        //});
        //$(".input-datepicker-close").datepicker({
        //    weekStart: 1
        //}).on("changeDate", function (e) {
        //    $(this).datepicker("hide")
        //});
        //$(".input-timepicker").timepicker({
        //    minuteStep: 1,
        //    showSeconds: true,
        //    showMeridian: true
        //});
        //$(".input-timepicker24").timepicker({
        //    minuteStep: 1,
        //    showSeconds: true,
        //    showMeridian: false
        //});
        $(".pie-chart").easyPieChart({
            barColor: $(this).data("bar-color") ? $(this).data("bar-color") : "#777777",
            trackColor: $(this).data("track-color") ? $(this).data("track-color") : "#eeeeee",
            lineWidth: $(this).data("line-width") ? $(this).data("line-width") : 3,
            size: $(this).data("size") ? $(this).data("size") : "80",
            animate: 800,
            scaleColor: false
        });
        $("input, textarea").placeholder()
    }
    ;
    var a = function () {
        var e = $("body");
        if (e.hasClass("page-loading")) {
            e.removeClass("page-loading")
        }
    }
    ;
    var f = function () {
        return window.innerWidth || document.documentElement.clientWidth || document.body.clientWidth
    }
    ;
    var l = function () {
        var e = 250;
        var t = 250;
        var n = $(".sidebar-nav-menu");
        var r = $(".sidebar-nav-submenu");
        n.click(function () {
            var n = $(this);
            if (n.parent().hasClass("active") !== true) {
                if (n.hasClass("open")) {
                    n.removeClass("open").next().slideUp(e, function () {
                        c(n, 200, 300)
                    });
                    setTimeout(v, e)
                } else {
                    $(".sidebar-nav-menu.open").removeClass("open").next().slideUp(e);
                    n.addClass("open").next().slideDown(t, function () {
                        c(n, 150, 600)
                    });
                    setTimeout(v, e > t ? e : t)
                }
            }
            return false
        });
        r.click(function () {
            var n = $(this);
            if (n.parent().hasClass("active") !== true) {
                if (n.hasClass("open")) {
                    n.removeClass("open").next().slideUp(e, function () {
                        c(n, 200, 300)
                    });
                    setTimeout(v, e)
                } else {
                    n.closest("ul").find(".sidebar-nav-submenu.open").removeClass("open").next().slideUp(e);
                    n.addClass("open").next().slideDown(t, function () {
                        c(n, 150, 600)
                    });
                    setTimeout(v, e > t ? e : t)
                }
            }
            return false
        })
    }
    ;
    var c = function (t, r, i) {
        if (!e.hasClass("disable-menu-autoscroll")) {
            var s;
            if (!n.hasClass("navbar-fixed-top") && !n.hasClass("navbar-fixed-bottom")) {
                var o = t.offset().top;
                s = o - r > 0 ? o - r : 0;
                $("html, body").animate({
                    scrollTop: s
                }, i)
            } else {
                var u = t.parents(".sidebar-scroll");
                var a = t.offset().top + Math.abs($("div:first", u).offset().top);
                s = a - r > 0 ? a - r : 0;
                u.animate({
                    scrollTop: s
                }, i)
            }
        }
    }
    ;
    var h = function (t, r) {
        if (t === "init") {
            if (n.hasClass("navbar-fixed-top") || n.hasClass("navbar-fixed-bottom")) {
                h("sidebar-scroll")
            }
            $(".sidebar-partial #sidebar").mouseenter(function () {
                h("close-sidebar-alt")
            });
            $(".sidebar-alt-partial #sidebar-alt").mouseenter(function () {
                h("close-sidebar")
            })
        } else {
            var i = f();
            if (t === "toggle-sidebar") {
                if (i > 991) {
                    e.toggleClass("sidebar-visible-lg");
                    if (e.hasClass("sidebar-visible-lg")) {
                        h("close-sidebar-alt")
                    }
                    if (r === "toggle-other") {
                        if (!e.hasClass("sidebar-visible-lg")) {
                            h("open-sidebar-alt")
                        }
                    }
                } else {
                    e.toggleClass("sidebar-visible-xs");
                    if (e.hasClass("sidebar-visible-xs")) {
                        h("close-sidebar-alt")
                    }
                }
            } else if (t === "toggle-sidebar-alt") {
                if (i > 991) {
                    e.toggleClass("sidebar-alt-visible-lg");
                    if (e.hasClass("sidebar-alt-visible-lg")) {
                        h("close-sidebar")
                    }
                    if (r === "toggle-other") {
                        if (!e.hasClass("sidebar-alt-visible-lg")) {
                            h("open-sidebar")
                        }
                    }
                } else {
                    e.toggleClass("sidebar-alt-visible-xs");
                    if (e.hasClass("sidebar-alt-visible-xs")) {
                        h("close-sidebar")
                    }
                }
            } else if (t === "open-sidebar") {
                if (i > 991) {
                    e.addClass("sidebar-visible-lg")
                } else {
                    e.addClass("sidebar-visible-xs")
                }
                h("close-sidebar-alt")
            } else if (t === "open-sidebar-alt") {
                if (i > 991) {
                    e.addClass("sidebar-alt-visible-lg")
                } else {
                    e.addClass("sidebar-alt-visible-xs")
                }
                h("close-sidebar")
            } else if (t === "close-sidebar") {
                if (i > 991) {
                    e.removeClass("sidebar-visible-lg")
                } else {
                    e.removeClass("sidebar-visible-xs")
                }
            } else if (t === "close-sidebar-alt") {
                if (i > 991) {
                    e.removeClass("sidebar-alt-visible-lg")
                } else {
                    e.removeClass("sidebar-alt-visible-xs")
                }
            } else if (t == "sidebar-scroll") {
                if (o.length && !o.parent(".slimScrollDiv").length) {
                    o.slimScroll({
                        height: $(window).height(),
                        color: "#fff",
                        size: "3px",
                        touchScrollStep: 100
                    });
                    $(window).resize(p);
                    $(window).bind("orientationchange", d)
                }
            }
        }
        return false
    }
    ;
    var p = function () {
        o.add(o.parent()).css("height", $(window).height())
    }
    ;
    var d = function () {
        setTimeout(o.add(o.parent()).css("height", $(window).height()), 500)
    }
    ;
    var v = function () {
        var o = $(window).height();
        var u = i.outerHeight();
        var a = s.outerHeight();
        var f = n.outerHeight();
        var l = r.outerHeight();
        if (n.hasClass("navbar-fixed-top") || n.hasClass("navbar-fixed-bottom") || u < o && a < o) {
            if (e.hasClass("footer-fixed")) {
                t.css("min-height", o - f + "px")
            } else {
                t.css("min-height", o - (f + l) + "px")
            }
        } else {
            if (e.hasClass("footer-fixed")) {
                t.css("min-height", (u > a ? u : a) - f + "px")
            } else {
                t.css("min-height", (u > a ? u : a) - (f + l) + "px")
            }
        }
    }
    ;
    var m = function () {
        $('[data-toggle="block-toggle-content"]').on("click", function () {
            var e = $(this).closest(".block").find(".block-content");
            if ($(this).hasClass("active")) {
                e.slideDown()
            } else {
                e.slideUp()
            }
            $(this).toggleClass("active")
        });
        $('[data-toggle="block-toggle-fullscreen"]').on("click", function () {
            var e = $(this).closest(".block");
            if ($(this).hasClass("active")) {
                e.removeClass("block-fullscreen")
            } else {
                e.addClass("block-fullscreen")
            }
            $(this).toggleClass("active")
        });
        $('[data-toggle="block-hide"]').on("click", function () {
            $(this).closest(".block").fadeOut()
        })
    }
    ;
    var g = function () {
        var e = $("#to-top");
        $(window).scroll(function () {
            if ($(this).scrollTop() > 150 && f() > 991) {
                e.fadeIn(100)
            } else {
                e.fadeOut(100)
            }
        });
        e.click(function () {
            $("html, body").animate({
                scrollTop: 0
            }, 400);
            return false
        })
    }
    ;
    //var y = function () {
    //    var e = $(".chat-users");
    //    var t = $(".chat-talk");
    //    var n = $(".chat-talk-messages");
    //    var r = $("#sidebar-chat-message");
    //    var i = "";
    //    $(".chat-talk-messages").slimScroll({
    //        height: 210,
    //        color: "#fff",
    //        size: "3px",
    //        position: "left",
    //        touchScrollStep: 100
    //    });
    //    $("a", e).click(function () {
    //        e.slideUp();
    //        t.slideDown();
    //        r.focus();
    //        return false
    //    });
    //    $("#chat-talk-close-btn").click(function () {
    //        t.slideUp();
    //        e.slideDown();
    //        return false
    //    });
    //    $("#sidebar-chat-form").submit(function (e) {
    //        i = r.val();
    //        if (i) {
    //            n.append('<li class="chat-talk-msg chat-talk-msg-highlight themed-border animation-slideLeft">' + $("<div />").text(i).html() + "</li>");
    //            n.animate({
    //                scrollTop: n[0].scrollHeight
    //            }, 500);
    //            r.val("")
    //        }
    //        e.preventDefault()
    //    })
    //}
    //;
    var b = function () {
        var t = $(".sidebar-themes");
        var r = $("#theme-link");
        var i;
        if (r.length) {
            i = r.attr("href");
            $("li", t).removeClass("active");
            $('a[data-theme="' + i + '"]', t).parent("li").addClass("active")
        }
        $("a", t).click(function (e) {
            i = $(this).data("theme");
            $("li", t).removeClass("active");
            $(this).parent("li").addClass("active");
            if (i === "default") {
                if (r.length) {
                    r.remove();
                    r = $("#theme-link")
                }
            } else {
                if (r.length) {
                    r.attr("href", i)
                } else {
                    $('link[href="css/themes.css"]').before('<link id="theme-link" rel="stylesheet" href="' + i + '">');
                    r = $("#theme-link")
                }
            }
        });
        $(".dropdown-options a").click(function (e) {
            e.stopPropagation()
        });
        var s = $("#options-main-style");
        var o = $("#options-main-style-alt");
        if (e.hasClass("style-alt")) {
            o.addClass("active")
        } else {
            s.addClass("active")
        }
        s.click(function () {
            e.removeClass("style-alt");
            $(this).addClass("active");
            o.removeClass("active")
        });
        o.click(function () {
            e.addClass("style-alt");
            $(this).addClass("active");
            s.removeClass("active")
        });
        var u = $("#options-header-default");
        var a = $("#options-header-inverse");
        var f = $("#options-header-top");
        var l = $("#options-header-bottom");
        if (n.hasClass("navbar-default")) {
            u.addClass("active")
        } else {
            a.addClass("active")
        }
        if (n.hasClass("navbar-fixed-top")) {
            f.addClass("active")
        } else if (n.hasClass("navbar-fixed-bottom")) {
            l.addClass("active")
        }
        u.click(function () {
            n.removeClass("navbar-inverse").addClass("navbar-default");
            $(this).addClass("active");
            a.removeClass("active")
        });
        a.click(function () {
            n.removeClass("navbar-default").addClass("navbar-inverse");
            $(this).addClass("active");
            u.removeClass("active")
        });
        f.click(function () {
            e.removeClass("header-fixed-bottom").addClass("header-fixed-top");
            n.removeClass("navbar-fixed-bottom").addClass("navbar-fixed-top");
            $(this).addClass("active");
            l.removeClass("active");
            h("sidebar-scroll");
            v()
        });
        l.click(function () {
            e.removeClass("header-fixed-top").addClass("header-fixed-bottom");
            n.removeClass("navbar-fixed-top").addClass("navbar-fixed-bottom");
            $(this).addClass("active");
            f.removeClass("active");
            h("sidebar-scroll");
            v()
        });
        var c = $("#options-footer-static");
        var p = $("#options-footer-fixed");
        if (e.hasClass("footer-fixed")) {
            p.addClass("active")
        } else {
            c.addClass("active")
        }
        c.click(function () {
            e.removeClass("footer-fixed");
            $(this).addClass("active");
            p.removeClass("active");
            v()
        });
        p.click(function () {
            e.addClass("footer-fixed");
            $(this).addClass("active");
            c.removeClass("active");
            v()
        })
    }
    ;
    var w = function () {
        $.extend(true, $.fn.dataTable.defaults, {
            sDom: "<'row'<'col-sm-6 col-xs-5'l><'col-sm-6 col-xs-7'f>r>t<'row'<'col-sm-5 hidden-xs'i><'col-sm-7 col-xs-12 clearfix'p>>",
            sPaginationType: "bootstrap",
            oLanguage: {
                sLengthMenu: "_MENU_",
                sSearch: '<div class="input-group">_INPUT_<span class="input-group-addon"><i class="fa fa-search"></i></span></div>',
                sInfo: "<strong>_START_</strong>-<strong>_END_</strong> of <strong>_TOTAL_</strong>",
                oPaginate: {
                    sPrevious: "",
                    sNext: ""
                }
            }
        });
        $.extend($.fn.dataTableExt.oStdClasses, {
            sWrapper: "dataTables_wrapper form-inline",
            sFilterInput: "form-control",
            sLengthSelect: "form-control"
        })
    }
    ;
    return {
        init: function () {
            u();
            a()
        },
        sidebar: function (e, t) {
            h(e, t)
        },
        datatables: function () {
            w()
        }
    }
}();
$(function () {
    App.init()
})
