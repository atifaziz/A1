# A1

[![Build Status][build-badge]][builds]
[![NuGet][nuget-badge]][nuget-pkg]
[![MyGet][myget-badge]][edge-pkgs]

A1 is a [.NET Standard][netstd] 1.0 library whose purpose is to help parse and
format A1-style cell references (or addresses) like `$FOO$42`, such as those
typically found in spreadsheet programs such as [Microsoft Excel][xl].

## Usage

Convert a column reference expressed as an _alphabet-string_ (one or more
characters from the alphabet) into a zero-based column offset:

```c#
var a = A1Convert.AlphaColumnNumber("A");
Console.WriteLine(a); // 0
```

Convert a zero-based column offset into an alphabet-string:

```c#
var foo = A1Convert.NumberColumnAlpha(4461)
Console.WriteLine(foo); // FOO
```

Query maximum column offset or alphabet-string supported:

```c#
Console.WriteLine(A1Convert.MaxColumn); // 16384
Console.WriteLine(A1Convert.NumberColumnAlpha(A1Convert.MaxColumn)); // XFD
```

There are two value types defined in the library:

- `ColRow`: simply represents a column and row pair.
- `Address`: builds on `ColRow` and permits flagging the column and/or row as
  being absolute or relative. It also provides parsing A1-style cell
  references into an `Address` instance.

Following is a demonstration of to use these types.

Initialize a `ColRow` then format in A1-style:

```c#
var xy = new ColRow(12, 34);
Console.WriteLine(xy);                            // (12,34)
Console.WriteLine(xy.FormatA1());                 // M35
```

`ColRow` values can be compared for equality and inequality:

```c#
Console.WriteLine(xy == new ColRow(34, 12)); // False
Console.WriteLine(xy != new ColRow(34, 12)); // True
Console.WriteLine(xy == new ColRow(12, 34)); // True
Console.WriteLine(xy != new ColRow(12, 34)); // False

```

A `ColRow` value can be tested for containment within a matrix defined by
a pair of `ColRow` values (left-top then right-bottom):

```c#
var xy = new ColRow(12, 34);
var lt = new ColRow(0, 0);
var rbSmall = new ColRow(10, 10);
var rbLarge = new ColRow(10, 10);
Console.WriteLine(xy.IsContainedIn(lt, rbSmall)); // False
Console.WriteLine(xy.IsContainedIn(lt, rbLarge)); // True
```

Determine offset to another `ColRow`:

```c#
var a = new ColRow(17, 5);
var b = new ColRow(19, 71);
var o = a.OffsetTo(b, (x, y) => new { X = x, Y = y });
Console.WriteLine(o.ToString()); // { X = 2, Y = 66 }
```

Determine size of the box formed by two `ColRow` values:

```c#
var a = new ColRow(17, 5);
var b = new ColRow(19, 71);
var s = a.Size(b, (w, h) => new { Width = w, Height = h });
Console.WriteLine(s.ToString()); // { X = 2, Y = 66 }
```

Initializing an `Address` and formatting:


```c#
Console.WriteLine(new Address(12, 34));               // M35
Console.WriteLine(new Address(12, 34, false));        // M35
Console.WriteLine(new Address(12, 34, true));         // $M$35
Console.WriteLine(new Address(false, 12, false, 34)); // M35
Console.WriteLine(new Address(true , 12, false, 34)); // $M35
Console.WriteLine(new Address(false, 12, true , 34)); // M$35
Console.WriteLine(new Address(true , 12, true , 34)); // $M$35
```

Parsing an A1-style reference into an `Address`:

```c#
var a = Address.ParseA1("M$35");
Console.WriteLine(a);           // M$35
Console.WriteLine(a.IsColAbs);  // True
Console.WriteLine(a.Col);       // 12
Console.WriteLine(a.IsRowAbs);  // True
Console.WriteLine(a.Row);       // 34
Console.WriteLine(a.ColRow);    // (12, 34)
```

Make an `Address` absolute and then relative again:

```c#
var m35 = Address.ParseA1("M35");
var abs = m35.MakeAbsolute();       // $M$35
Console.WriteLine(abs);
var rel = abs.MakeRelative();       // M35
Console.WriteLine(rel);
```

Parse a range into an `Address` couple (i.e. `Tuple<Address, Address>`):

```c#
var range = Address.ParseA1Range("B5:Z10");
Console.WriteLine(range); // (B5, Z10)
```

Parse a range into an anonymous type holding the `Address` couple:

```c#
var range = Address.ParseA1Range("B5:Z10", (a, b) => new { From = a, To = b });
Console.WriteLine(range.ToString()); // (B5, Z10)
```

`Address` values can be compared for equality and inequality:

```c#
var range = Address.ParseA1Range("B5:Z10");
Console.WriteLine(range.Item1 == range.Item2); // False
Console.WriteLine(range.Item1 != range.Item2); // True
```


  [netstd]: https://docs.microsoft.com/en-us/dotnet/articles/standard/library
  [xl]: https://www.microsoft.com/excel
  [build-badge]: https://img.shields.io/appveyor/ci/raboof/a1.svg
  [myget-badge]: https://img.shields.io/myget/raboof/v/A1.svg?label=myget
  [edge-pkgs]: https://www.myget.org/feed/raboof/package/nuget/A1
  [nuget-badge]: https://img.shields.io/nuget/v/A1.svg
  [nuget-pkg]: https://www.nuget.org/packages/A1
  [builds]: https://ci.appveyor.com/project/raboof/a1
