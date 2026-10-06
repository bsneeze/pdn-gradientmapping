# Gradient Mapping release notes

## Next version

### Fixed
- Presets could load with colors at the wrong positions when several colors sat close together ([#2](https://github.com/bsneeze/pdn-gradientmapping/issues/2)). Presets already re-saved while scrambled need to be recreated.
- Hard edges (two colors at the same position) blended toward the wrong color on the left side.
- Fading to a transparent color no longer darkens the gradient on the way there.
- Hue input was slightly off for colors between magenta and red.
- Reopening the effect after applying a preset showed "Custom" instead of the preset's name.
- Preset names containing a dot were cut off at the dot (saving "v1.5" produced "v1").
- A failed preset save could leave the preset dropdown unresponsive until the dialog was reopened.
- Cancelling "Save current as preset..." could leave the preset dropdown blank.
- Crash when the dialog lost focus while dragging a color.
- Crash when using the number keys to select a color that doesn't exist, then opening the menu or removing it.
- New colors are now added where you click, instead of slightly to the right.
- Cancelling "Change Color" no longer switches the preset to Custom.
- In the color picker, dragging one slider could shift the other channels by 1.
- Several memory and handle leaks in the dialog, color picker and preset list.

### Changed
- Built-in presets you delete now stay deleted. Any you deleted before this update will come back once; delete them again and they're gone for good.
- A preset file that can't be read now appears in the list as "(couldn't load)". Select it to see why.
- "Custom" can no longer be used as a preset name.
- Only `.xml` files in the preset folder are treated as presets.
- An empty gradient now leaves the image unchanged.
- With Preserve Alpha on, a fully transparent color in the gradient no longer contributes its own RGB to the colors around it. Presets that relied on this will look different.
- Faster rendering on large images.
