﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DownUnder.UI.UI.Widgets.DataTypes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using static System.Reflection.BindingFlags;

// Todo: Duplicate RectanglF extensions for Rectangle
// Todo: Turn some extensions to properties when C# 8 arrives

namespace DownUnder.UI {
    public static class Extensions {
        /// <summary> Returns true if this <see cref="Point2"/>'s X or Y field is larger than the given one's. </summary>
        public static bool IsLargerThan(this Point2 p, Point2 p2) =>
            p.X > p2.X || p.Y > p2.Y;

        /// <summary> Returns true if either X or Y is negative. </summary>
        public static bool IsNegative(this Point2 p) =>
            p.X < 0f || p.Y < 0f;

        /// <summary> Returns a new <see cref="Point2"/> with the highest X and Y values of both given Points. </summary>
        public static Point2 Max(this Point2 p, Point2 p2) =>
            new Point2(MathHelper.Max(p.X, p2.X), MathHelper.Max(p.Y, p2.Y));

        /// <summary> Returns the higher X or Y value. </summary>
        public static float MaxFloat(this Point2 p) =>
            MathHelper.Max(p.X, p.Y);

        /// <summary> Returns a new <see cref="Point2"/> with the lowest X and Y values of both given Points. </summary>
        public static Point2 Min(this Point2 p, Point2 p2) =>
            new Point2(MathHelper.Min(p.X, p2.X), MathHelper.Min(p.Y, p2.Y));

        /// <summary> Returns the lower X or Y value. </summary>
        public static float MinFloat(this Point2 p) =>
            MathHelper.Min(p.X, p.Y);

        public static Point2 MultipliedBy(this Point2 p, float scale) =>
            new Point2(p.X * scale, p.Y * scale);

        public static Point2 MultipliedBy(this Point2 p, Point2 p2) =>
            new Point2(p.X * p2.X, p.Y * p2.Y);

        public static Point2 DividedBy(this Point2 p, float scale) =>
            new Point2(p.X / scale, p.Y / scale);

        public static float Product(this Point2 p) =>
            p.X * p.Y;

        public static double DistanceFrom(this Point2 p1, Point2 p2) =>
            Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));

        public static double DistanceFrom(this Point2 p, RectangleF r) =>
            r.DistanceFrom(p);

        public static Point2 DividedBy(this Point2 p, Point2 p2) =>
            new Point2(p.X / p2.X, p.Y / p2.Y);

        public static float LesserValue(this Point2 p) =>
            MathHelper.Min(p.X, p.Y);

        public static float GreaterValue(this Point2 p) =>
            MathHelper.Max(p.X, p.Y);

        public static Point2 XOnly(this Point2 p) => new Point2(p.X, 0f);
        public static Point2 YOnly(this Point2 p) => new Point2(0f, p.Y);
        public static Point2 XOnly(this Vector2 p) => new Vector2(p.X, 0f);
        public static Point2 YOnly(this Vector2 p) => new Vector2(0f, p.Y);

        /// <summary> Performs linear interpolation of <see cref="Point2"/>. </summary>
        /// <param name="progress">Value between 0f and 1f to represent the progress between the two <see cref="Point2"/>s</param>
        public static Point2 Lerp(
            this Point2 p1,
            Point2 p2,
            float progress
        ) {
            var inverse_progress = 1 - progress;

            return new Point2(
                p1.X * inverse_progress + p2.X * progress,
                p1.Y * inverse_progress + p2.Y * progress
            );
        }

        /// <summary> Returns new <see cref="Color"/> with brightness shifted by a percentage. </summary>
        public static Color ShiftBrightness(
            this Color c,
            float percentage
        ) =>
            percentage > 1
                ? new Color(
                    (byte)(c.R + (255 - c.R) * (percentage - 1)),
                    (byte)(c.G + (255 - c.G) * (percentage - 1)),
                    (byte)(c.B + (255 - c.B) * (percentage - 1)),
                    c.A
                )
                : new Color(
                    (byte)(c.R * percentage),
                    (byte)(c.G * percentage),
                    (byte)(c.B * percentage),
                    c.A
                );

        public static Color WithR(this Color c, byte r) => new Color(r, c.G, c.B, c.A);
        public static Color WithG(this Color c, byte g) => new Color(c.R, g, c.B, c.A);
        public static Color WithB(this Color c, byte b) => new Color(c.R, c.G, b, c.A);
        public static Color WithA(this Color c, byte a) => new Color(c.R, c.G, c.B, a);

        /// <summary> Returns a <see cref="RectangleF"/> to represent each line of text. </summary>
        public static List<RectangleF> MeasureStringAreas(
            this SpriteFont sprite_font,
            string text
        ) =>
            MeasureTrimmedString(sprite_font, text, 0, 0);

        /// <summary> Returns a <see cref="RectangleF"/> to represent each line of text for a selected substring of the text. </summary>
        public static List<RectangleF> MeasureSubStringAreas(
            this SpriteFont sprite_font,
            string text,
            int index,
            int length,
            bool debug = false
        ) =>
            MeasureTrimmedString(sprite_font, text, index, text.Length - length - index, debug);

        /// <summary> Returns a <see cref="RectangleF"/> to represent each line of text for a trimmed version of the text. </summary>
        /// <param name="ltrim">How many characters should be removed from the start of the string.</param>
        /// <param name="rtrim">How many characters should be removed from the end of the string.</param>
        public static List<RectangleF> MeasureTrimmedString(
            this SpriteFont sprite_font,
            string text,
            int ltrim,
            int rtrim,
            bool debug = false
        ) {
            var result = new List<RectangleF>();

            var area = new RectangleF();
            var y = 0f;
            var length_processed = 0;
            var initial = true;

            foreach (var line in text.Split('\n').ToList()) {
                if (length_processed + line.Length >= ltrim) {
                    // If this is the line where the start_trim is
                    if (initial && ltrim != 0) {
                        initial = false;
                        area = MeasureSingleLineSubString(sprite_font, line, ltrim - length_processed, line.Length - (ltrim - length_processed));
                        area.Y = y;
                    } else {
                        area.X = 0f;
                        area.Size = sprite_font.MeasureString(line);
                    }

                    // If this is the line where the end trim is
                    if (line.Length + length_processed >= text.Length - rtrim) {
                        area.Width -= sprite_font.MeasureString(text[^rtrim..].Split(new char[] { '\n' })[0]).X;
                        result.Add(area);
                        break;
                    }

                    result.Add(area);
                }

                y += sprite_font.MeasureString("|").Y;
                area.Y = y;
                length_processed += line.Length + 1; // Add one to account for the removed \n
            }

            return result;
        }


        /// <summary> Returns the area of a single substring. A method used above. </summary>
        static RectangleF MeasureSingleLineSubString(
            SpriteFont sprite_font,
            string line,
            int index,
            int length
        ) =>
            new RectangleF(
                sprite_font.MeasureString(line.Substring(0, index)).X,
                0f,
                sprite_font.MeasureString(line.Substring(index, length)).X,
                sprite_font.MeasureString("|").Y
            );

        /// <summary> Returns the character index in a spacial point in a string. </summary>
        public static int IndexFromPoint(
            this SpriteFont sprite_font,
            string text,
            Point2 point,
            bool round_up = false
        ) {
            // Calculate the line the char is on
            var line_height = sprite_font.MeasureString("|").Y;
            var lines = text.Split('\n').ToList();
            int line_y;
            if (point.Y < 0)
                line_y = 0;
            else {
                line_y = (int)(point.Y / line_height);

                if (line_y >= lines.Count)
                    line_y = lines.Count - 1;
            }

            // Calculate the index of the line the char is on
            var line_x = 0;
            var counted_length = 0f;
            foreach (var c in lines[line_y]) {
                var char_length = sprite_font.MeasureString(c.ToString()).X;
                counted_length += char_length;

                if (round_up) {
                    if (point.X + char_length / 2 < counted_length)
                        break;
                } else if (point.X + char_length < counted_length)
                    break;

                line_x++;
            }

            // Calculate the index given the line_x and the line_y
            var result = 0;
            for (var i = 0; i < line_y; i++)
                result += lines[i].Length + 1; // Add one for the removed /n
            result += line_x;

            return result;
        }

        /// <summary> Get the position of a character in a string. </summary>
        public static Vector2 GetCharacterPosition(this SpriteFont sprite_font, string text, int index) =>
            MeasureTrimmedString(sprite_font, text, index, 0)[0].Position;

        ///// <summary> Converts Monogame Texture2D to System.Drawing.Image. </summary>
        ///// <param name="texture">Texture2D to convert.</param>
        ///// <returns>Converted Image.</returns>
        //public static System.Drawing.Image ToImage(this Texture2D texture)
        //{
        //    if (texture == null) { return null; }

        //    byte[] textureData = new byte[4 * texture.Width * texture.Height];
        //    texture.GetData<byte>(textureData);

        //    System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(texture.Width, texture.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        //    System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(new System.Drawing.Rectangle(0, 0, texture.Width, texture.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        //    IntPtr safePtr = bmpData.Scan0;

        //    System.Runtime.InteropServices.Marshal.Copy(textureData, 0, safePtr, textureData.Length);
        //    bmp.UnlockBits(bmpData);

        //    return bmp;
        //}

        /// <summary> Returns new Point2 with the sum of each. </summary>
        public static Point2 WithOffset(this Point2 p1, Point2 p2) =>
            new Point2(p1.X + p2.X, p1.Y + p2.Y);

        public static Point2 WithXOffset(this Point2 p, float x) =>
            new Point2(p.X + x, p.Y);

        public static Point2 WithYOffset(this Point2 p, float y) =>
            new Point2(p.X, p.Y + y);

        public enum NumericRelationship {
            GreaterThan = 1,
            EqualTo = 0,
            LessThan = -1
        };

        //public static NumericRelationship Compare(this ValueType value1, ValueType value2)
        //{
        //    if (!IsNumeric(value1))
        //        throw new ArgumentException("value1 is not a number.");
        //    else if (!IsNumeric(value2))
        //        throw new ArgumentException("value2 is not a number.");

        //    // Use BigInteger as common integral type
        //    if (IsInteger(value1) && IsInteger(value2))
        //    {
        //        System.Numerics.BigInteger bigint1 = (System.Numerics.BigInteger)value1;
        //        System.Numerics.BigInteger bigint2 = (System.Numerics.BigInteger)value2;
        //        return (NumericRelationship)System.Numerics.BigInteger.Compare(bigint1, bigint2);
        //    }
        //    // At least one value is floating point; use Double.
        //    else
        //    {
        //        Double dbl1 = 0;
        //        Double dbl2 = 0;
        //        try
        //        {
        //            dbl1 = Convert.ToDouble(value1);
        //        }
        //        catch (OverflowException)
        //        {
        //            Console.WriteLine("value1 is outside the range of a Double.");
        //        }
        //        try
        //        {
        //            dbl2 = Convert.ToDouble(value2);
        //        }
        //        catch (OverflowException)
        //        {
        //            Console.WriteLine("value2 is outside the range of a Double.");
        //        }
        //        return (NumericRelationship)dbl1.CompareTo(dbl2);
        //    }
        //}

        public static bool IsInteger(
            this ValueType value
        ) =>
            value is sbyte || value is short || value is int
            || value is long || value is byte || value is ushort
            || value is uint || value is ulong;
                    //|| value is System.Numerics.BigInteger);

        public static bool IsNumeric(
            this ValueType value
        ) =>
            value is byte ||
            value is short ||
            value is int ||
            value is long ||
            value is sbyte ||
            value is ushort ||
            value is uint ||
            value is ulong ||
            //value is System.Numerics.BigInteger ||
            value is decimal ||
            value is double ||
            value is float;

        public static bool IsFloat(this ValueType value) =>
            value is float
            || value is double
            || value is decimal;

        static readonly HashSet<Type> NumericTypes = new HashSet<Type> {
            typeof(int),  typeof(double),  typeof(decimal),
            typeof(long), typeof(short),   typeof(sbyte),
            typeof(byte), typeof(ulong),   typeof(ushort),
            typeof(uint), typeof(float)
        };

        public static bool IsNumeric(
            this Type type
        ) =>
            NumericTypes.Contains(type)
            || NumericTypes.Contains(Nullable.GetUnderlyingType(type));

        static readonly HashSet<Type> NonIntegralTypes = new HashSet<Type> {
            typeof(float),  typeof(double),  typeof(decimal)
        };

        internal static bool IsIntegral(this Type type) =>
            !(NonIntegralTypes.Contains(type)
            || NonIntegralTypes.Contains(Nullable.GetUnderlyingType(type)));

        public static IEnumerable<T> OrEmptyIfNull<T>(
            this IEnumerable<T> source
        ) =>
            source ?? Enumerable.Empty<T>();

        public static BorderSize Difference(
            this RectangleF r1,
            RectangleF r2
        ) =>
            new BorderSize(r1.Y - r2.Y, r1.Bottom - r2.Bottom, r1.X - r2.X, r1.Right - r2.Right);

        // https://stackoverflow.com/questions/12680341/how-to-get-both-fields-and-properties-in-single-call-via-reflection
        public static MemberInfo[] GetEditableMembers(
            this Type type
        ) =>
            type
            .GetFields(Public | Instance)
            .Cast<MemberInfo>()
            .Concat(type.GetProperties(Public | Instance))
            .ToArray();

        //https://stackoverflow.com/questions/238555/how-do-i-get-the-value-of-memberinfo
        public static object GetValue(
            this MemberInfo memberInfo,
            object forObject
        ) =>
            memberInfo.MemberType switch {
                MemberTypes.Field => ((FieldInfo)memberInfo).GetValue(forObject),
                MemberTypes.Property => ((PropertyInfo)memberInfo).GetValue(forObject),
                _ => throw new NotSupportedException("Member must be a Field or Property.")
            };


        public static ParameterInfo[] GetParameters(
            this MemberInfo memberInfo
        ) =>
            memberInfo.MemberType switch {
                MemberTypes.Field => new ParameterInfo[0],
                MemberTypes.Property => ((PropertyInfo)memberInfo).GetIndexParameters(),
                _ => throw new NotSupportedException("Member must be a Field or Property.")
            };

        public static IEnumerable<T> FastReverse<T>(
            this IList<T> items
        ) {
            for (var i = items.Count - 1; i >= 0; i--)
                yield return items[i];
        }

        /// <summary> Returns a new <see cref="Vector2"/> with a -X value. </summary>
        public static Vector2 InvertX(this Vector2 v) => new Vector2(-v.X, v.Y);

        /// <summary> Returns a new <see cref="Vector2"/> with a -Y value. </summary>
        public static Vector2 InvertY(this Vector2 v) => new Vector2(v.X, -v.Y);

        public static VertexPositionColor Lerp(
            this VertexPositionColor vpc,
            VertexPositionColor target,
            float amount
        ) =>
            new VertexPositionColor(
                Vector3.Lerp(vpc.Position, target.Position, amount),
                Color.Lerp(vpc.Color, target.Color, amount)
            );

        public static Size2 ToSize2(this Point2 p) => new Size2(p.X, p.Y);
    }
}
