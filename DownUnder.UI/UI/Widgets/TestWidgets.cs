﻿using System;
using System.Diagnostics;
using DownUnder.UI.UI.Widgets.Actions.Functional;
using DownUnder.UI.UI.Widgets.Behaviors.Format;
using DownUnder.UI.UI.Widgets.Behaviors.Functional;
using DownUnder.UI.UI.Widgets.Behaviors.Visual;
using DownUnder.UI.UI.Widgets.DataTypes;
using DownUnder.UI.UI.Widgets.DataTypes.AnimatedGraphics;
using DownUnder.UI.UI.Widgets.Signals;
using DownUnder.UI.Utilities.CommonNamespace;
using MonoGame.Extended;

namespace DownUnder.UI.UI.Widgets {
    public static class TestWidgets {
        public static Widget CurrentTest =>
            //WidgetReorderTest();
            //WidgetTransitionTest();
            //LoginLayoutEffects();
            //BGAnimationTest();
            //GridDebug();
            //GraphicTest();
            SpacedGridTest();

        public static Widget SpacedGridTest() {
            var result = new Widget();
            result.Add(new Widget(new RectangleF(10, 10, 200, 200)).WithAddedBehavior(new GridFormat(2, 3, null, new Point2(10,10)), out var grid_format), out var grid_widget);
            grid_widget.UserResizePolicy = Widget.UserResizePolicyType.allow;
            //grid_widget[0, 0].MinimumHeight = 30f;
            grid_widget[0, 0].IsFixedHeight = true;
            return result;
        }

        public static Widget GraphicTest() {
            var result = new Widget();
            result.Add(new Widget().WithAddedBehavior(SwitchingGraphic.PausePlayGraphic(), out var pause_play), out var inner);
            inner.Area = new RectangleF(50, 50, 650, 300);
            inner.UserResizePolicy = Widget.UserResizePolicyType.allow;
            inner.VisualSettings.DrawBackground = false;
            inner.VisualSettings.DrawOutline = false;
            //pause_play.Graphic.Progress.InterpolationSettings = InterpolationSettings.Faster;
            //inner.UserRepositionPolicy = Widget.UserResizePolicyType.allow;
            return result;
        }

        public static Widget GridDebug() {
            var result = new Widget();
            var grid_widget = new Widget().WithAddedBehavior(new GridFormat(1, 4));
            grid_widget[0].debug_output = true;
            grid_widget.Area = new RectangleF(50, 50, 100, 75);

            grid_widget[0, 0].Behaviors.Add(new DrawText("Test text 55555555555 5"));
            grid_widget[0, 1].Behaviors.Add(new DrawText("Test text 55555555555 5"));

            grid_widget.UserResizePolicy = Widget.UserResizePolicyType.allow;
            grid_widget.AllowedResizingDirections = Directions2D.All;
            result.Add(grid_widget);

            var second = new Widget();
            second.Area = new RectangleF(50, 150, 100, 75);
            second.Behaviors.Add(new GridFormat(3, 3, new Widget { UserResizePolicy = Widget.UserResizePolicyType.allow }));

            result.Add(second);

            return result;
        }

        public static Widget WidgetReorderTest() {
            var result = new Widget();
            Widget box1 = new Widget();
            Widget box2 = new Widget();
            box1.Area = new RectangleF(50,50,100,100);
            box2.Area = new RectangleF(100,100,100,100);

            result.Add(box1);
            result.Add(box2);

            box1.OnClick += (s, a) => box1.SendToFront();
            box2.OnClick += (s, a) => box2.SendToFront();

            return result;
        }

        public static Widget WidgetTransitionTest() {
            var result = new Widget();
            var button = CommonWidgets.Button("Send to container");
            var test_widget = new Widget {
                Area = new RectangleF(10, 40, 200, 200)
            };
            test_widget.Behaviors.Add(new GridFormat(2, 2));

            button.OnClick += (s, a) => test_widget.Actions.Add(new ReplaceWidget(new Widget().WithAddedBehavior(new DrawText("New Widget")), InnerWidgetLocation.OutsideLeft, InnerWidgetLocation.OutsideRight));

            result.Add(button);
            result.Add(test_widget);
            return result;
        }

        public static Widget CenterTest() {
            var layout = new Widget();
            layout.Behaviors.Add(new CenterContent());

            var inner = new Widget { Size = new Point2(300, 200) };
            var second_inner = new Widget { Position = new Point2(350, 40) };
            var third_inner = new Widget { Position = new Point2(600, 40) };

            layout.Add(inner);
            layout.Add(second_inner);
            layout.Add(third_inner);

            return layout;
        }

        public static Widget LoginLayout(
            Action<LoginSignal> handle_login,
            Action<CreateAccountSignal> handle_account_creation = null
        ) {
            var layout = new Widget { };
            layout.VisualSettings.ChangeColorOnMouseOver = false;
            layout.VisualSettings.DrawBackground = false;

            var login_button = CommonWidgets.Button("Login");
            login_button.Position = new Point2(20, 20);
            login_button.Size = new Point2(100, 40);
            login_button.OnClick += (s, a) => {
                if (layout["Login Window"] == null) layout.Add(LoginWindow(handle_login, handle_account_creation));
            };

            layout.Add(login_button);
            layout.Behaviors.Add(new Neurons());
            layout.Behaviors.Add(ShadingBehavior.GlowingGreen);

            return layout;
        }

        public static Widget LoginWindow(
            Action<LoginSignal> confirm_handle,
            Action<CreateAccountSignal> handle_account_creation = null
        ) {
            const int login_create_spacing = 8 / 2;

            var window = new Widget { Size = new Point2(400, 300), Name = "Login Window" };
            window.VisualSettings.VisualRole = GeneralVisualSettings.VisualRoleType.pop_up;

            window.Behaviors.Add(new CenterContent());
            window.Behaviors.Add(new PinWidget { Pin = InnerWidgetLocation.Centered });
            window.Behaviors.Add(new PopInOut(RectanglePart.Uniform(0.975f), RectanglePart.Uniform(0.5f)) { OpeningMotion = InterpolationSettings.Fast, ClosingMotion = InterpolationSettings.Fast });

            var username_entry = CommonWidgets.SingleLineTextEntry("", DrawText.XTextPositioningPolicy.left, DrawText.YTextPositioningPolicy.center, 8f);
            var password_entry = CommonWidgets.SingleLineTextEntry("", DrawText.XTextPositioningPolicy.left, DrawText.YTextPositioningPolicy.center, 8f);

            username_entry.Area = new RectangleF(100, 0, 150, 40);
            password_entry.Area = new RectangleF(100, 60, 150, 40);

            var username_label = CommonWidgets.Label("Username:");
            username_label.Area = new RectangleF(0, 0, 60, 40);

            var password_label = CommonWidgets.Label("Password:");
            password_label.Area= new RectangleF(0, 60, 60, 40 );

            var login_button = CommonWidgets.Button("Login");

            login_button.Area = handle_account_creation != null
                ? new RectangleF(125 + login_create_spacing, 120, 125 - login_create_spacing, 40)
                : new RectangleF(100, 120, 150, 40);

            window.Add(username_label);
            window.Add(password_label);
            window.Add(username_entry);
            window.Add(password_entry);
            window.Add(login_button);

            void handleResponse(WidgetResponse response) {
                if (response.Reply != WidgetResponse.ResponseType.reject)
                    return;

                window.ParentDWindow.ShowPopUpMessage("Could not log in.\n\nError: " + response.Message);
            }

            login_button.OnClick += (s, a) =>
                confirm_handle.Invoke(new LoginSignal(
                    username_entry.Behaviors.Common.DrawText.Text,
                    password_entry.Behaviors.Common.DrawText.Text,
                    handleResponse
                ));

            if (handle_account_creation == null)
                return window;

            var create_account_button = CommonWidgets.Button("Create Account");
            create_account_button.Area = new RectangleF(0, 120, 125 - login_create_spacing, 40);

            void handleCreationResponse(WidgetResponse r) {
                if (r.Reply != WidgetResponse.ResponseType.reject)
                    return;

                window.ParentDWindow.ShowPopUpMessage("Error creating account: " + r.Message);
            }

            create_account_button.OnClick += (s, a) =>
                handle_account_creation.Invoke(
                    new CreateAccountSignal(
                        username_entry.Behaviors.Common.DrawText.Text,
                        password_entry.Behaviors.Common.DrawText.Text,
                        "",
                        handleCreationResponse
                    )
                );

            window.Add(create_account_button);

            return window;
        }

        public static Widget BGAnimationTest() {
            var result = new Widget();
            result.Behaviors.Add(new DrawText { Text = "Click me", XTextPositioning = DrawText.XTextPositioningPolicy.center, YTextPositioning = DrawText.YTextPositioningPolicy.center });
            result.Behaviors.Add(new Neurons());
            return result;
        }

        public static Widget SerializeTest() {
            var result = new Widget();

            result.Add(CommonWidgets.Button("serialize", new RectangleF(10,10,100,40)), out var button);
            result.Add(CommonWidgets.Label("Native:", new RectangleF(20,70,200,200), DrawText.XTextPositioningPolicy.left, DrawText.YTextPositioningPolicy.top));
            result.Add(CommonWidgets.Label("Loaded from XML:", new RectangleF(20, 220, 200, 200), DrawText.XTextPositioningPolicy.left, DrawText.YTextPositioningPolicy.top));

            button.OnClick += (s, a) => {
                //var test = new TestSerializeClass() { NameProperty = "property name", name_field = "name field" };
                //test.w_list.Add(new Widget());
                //XmlHelper.ToXmlFile(test, @"C:\Users\jamie\Desktop\New folder\test.xml");
                var unserialized = new Widget(170, 70, 140, 140);
                unserialized.Add(new Widget(40,40,100,40), out var added_widget);
                added_widget.UserResizePolicy = Widget.UserResizePolicyType.allow;
                added_widget.Behaviors.Add(new GridFormat(2, 2), out var grid);
                grid[0, 1].Behaviors.Add(new DrawText("stuff"));
                unserialized.Behaviors.Add(new DrawText("Serialize this"));
                unserialized.SaveToXML(@"C:\Users\jamie\Desktop\New folder\test.xml");
                var deserialized = Widget.LoadFromXML(@"C:\Users\jamie\Desktop\New folder\test.xml");
                result.Add(deserialized);
                result.Add(unserialized);
                deserialized.Position = new Point2(170, 220);
            };

            return result;
        }

        public static Widget ExplorerTest() {
            var result = new Widget();
            result.Add(CommonWidgets.Explorer(@"C:\Users\jamie\Desktop\ii"), out var explorer);
            explorer.Position = new Point2(50, 50);
            return result;
        }

        public static Widget BrokenStuff() {
            var result = new Widget();
            var inner = new Widget();
            inner.Position = new Point2(50, 50);
            inner.Add(CommonWidgets.Label("Testgfd"));
            inner.Add(CommonWidgets.Label("Testfdasfdsa"));
            inner.Add(CommonWidgets.Label("Test"));
            inner.Add(CommonWidgets.Label("Test"));
            //inner.Add(CommonWidgets.Label("Test"));
            inner.Behaviors.Add(new GridFormat(1, inner.Count));
            result.Add(inner);
            return result;
        }

        public static Widget BrokenFixedSize() {
            var result = new Widget();
            var inner = new Widget();
            inner.Position = new Point2(50, 50);
            inner.Add(new Widget { IsFixedWidth = true });
            inner.Add(new Widget());

            inner.Behaviors.Add(new GridFormat(2, 1));

            Debug.WriteLine("Setting width");
            inner.Width = 100;

            //throw new Exception();

            result.Add(inner);
            return result;
        }
    }
}
