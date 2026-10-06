# Gradient Mapping

A [Paint.NET](https://www.paint.net/) plugin that recolors an image through a gradient. Each pixel's brightness (or another channel you pick) selects a color from the gradient, so you can tint photos, build duotones, make thermal or sepia looks, or generate multi-colored gradients from a plain black-to-white one.

Forum thread: https://forums.paint.net/index.php?/topic/6265-gradient-mapping/

## Installing

1. Close Paint.NET.
2. Copy `Gradient Mapping.dll` into `Documents\paint.net App Files\Effects\`. Create the folder if it doesn't exist.
3. Start Paint.NET. The effect is under **Adjustments > Gradient Mapping**.

This version is built for Paint.NET 5.1.

## Using it

The bar at the top of the dialog is the gradient. The triangles above and below it are its colors.

| To | Do this |
| --- | --- |
| Add a color | Double-click an empty part of the bar, or right-click and choose **Add Color** |
| Change a color | Double-click its triangle, or right-click it and choose **Change Color** |
| Move a color | Drag its triangle |
| Remove a color | Right-click its triangle and choose **Remove** |
| Space the colors evenly | Right-click and choose **Spread** |
| Flip the gradient | Right-click and choose **Reverse** |
| Start over with black to white | Right-click and choose **Clear** |

Two colors at the same position make a hard edge instead of a blend.

### Settings

- **Source** is the channel that selects a color from the gradient: Luminosity, Red, Green, Blue, Alpha, Cyan, Magenta, Yellow, Key / Black, Hue, Saturation, or Value / Brightness.
- **Offset** shifts every pixel along the gradient by up to 255 steps in either direction.
- **Wrap** makes an offset that runs off one end of the gradient continue from the other end. With it off, the offset stops at the end.
- **Preserve Alpha** keeps each pixel's original transparency and takes only the color from the gradient. With it off, transparency in the gradient is applied to the image.

### Keyboard

Click the gradient bar or tab to it first.

| Key | Action |
| --- | --- |
| `1` to `9`, `0` | Select the 1st to 10th color |
| `Tab` or `+` | Select the next color |
| `Shift+Tab` or `-` | Select the previous color |
| `Left` / `Right` | Move the selected color by one pixel |
| `Up` / `Down` | Make the selected color more or less opaque |
| `Ctrl` with an arrow key | Move or change opacity in bigger steps |
| `Enter` | Change the selected color |
| Menu key | Open the right-click menu |

## Presets

The dropdown under the gradient lists your presets. A preset stores the gradient and all four settings.

- **Save current as preset...** saves what's in the dialog under a name you choose.
- **Manage presets...** opens the preset folder, where you can rename, copy, or delete presets. The list updates while the dialog is open.
- A preset named `Default` replaces the gradient the dialog starts with.
- A preset that can't be read is listed as "(couldn't load)". Select it to see why.

Presets are `.xml` files in `Documents\Paint.NET User Files\Effect Presets\Gradient Mapping\`. They can be shared by copying the files, and they're simple enough to edit by hand:

```xml
<?xml version="1.0" encoding="utf-8"?>
<Gradient InputChannel="L" Offset="0" Wrap="true" LockAlpha="false">
  <Colors>
    <ColorBgra B="0" G="0" R="0" A="255" />
    <ColorBgra B="0" G="0" R="255" A="255" />
    <ColorBgra B="255" G="255" R="255" A="255" />
  </Colors>
  <Positions>
    <double>0</double>
    <double>0.75</double>
    <double>1</double>
  </Positions>
</Gradient>
```

`InputChannel` is one of `L`, `R`, `G`, `B`, `A`, `C`, `M`, `Y`, `K`, `H`, `S`, `V`, and `LockAlpha` is the Preserve Alpha setting. Each position goes with the color at the same place in the list and runs from 0 (left) to 1 (right). If `Positions` is left out, the colors are spaced evenly.

The plugin installs a set of built-in presets the first time it runs. If you delete one, it stays deleted.

## Building

You need the .NET SDK and Paint.NET installed in `C:\Program Files\Paint.NET`, which is where the project looks for the Paint.NET assemblies.

```
dotnet build
```

The build copies the plugin to `Documents\paint.net App Files\Effects\`. Close Paint.NET first, or the copy fails because the DLL is in use. Debug builds show up in Paint.NET as "Gradient Mapping BETA".

### Tests

```
dotnet test
```

The tests cover the gradient, color blending, channel mapping, and preset files. They need the .NET 10 runtime. The dialog and its controls aren't covered and have to be checked by hand in Paint.NET.

## License

[MIT](LICENSE)
