# Bachelor Final Projects
My final projects, 2 WPF psychological testing applications:

  - Dot-probe
  - Working memory capacity - continues encoding

# CustomControls - the cool stuff

## ColorPicker

This is the best custom control made for these projects.
It uses a custom shader to render the whole control.
The shader is **not optimized** at all, but is practical for this project.
Please do not try to learn a lot from my shader, using if isn't a great practice for shaders, lookup SIMD and if statements.

### Demo

TODO

### Anti-aliasing without multisampling

The anti-aliasing is done by blurring the edges with a thickness of: one pixel in a given direction.

This is implemented using equations desinged only for this control, hard coded inside the shader.

The method is not computationally expensive and generates perfectly smooth edges.

## Animations

Animations are done by calling animate async functions which take a token,
an update function and other parameters.

Each variable/element uses a token to animate,
a token is a pointer to an integer which increments on each animate call,
this allows the new process to stop the previous animate process to avoid conflicts.
Each call, overrides the previous ongoing animation.

An update function is a function which is called by each animation update,
to do actions based on the updated value that is being animated.
