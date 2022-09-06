# Chef (in Development)
A lightweight interpreted programming language for logic chip emulating

The language is really easy to use!

Source Code Files always end in **.chef**.

yourChipName.chef:
```
yourChipName(inputBit1, inputBit2){
  and(inputBit1, inputBit2)
}
```

Then just use the arguments to interpret your file.

```
chef.exe path/to/chip 1 1
```

This would return **True**

You can also import custom chips:

otherChip.chef:
```
import: "path/to/yourChipName.chef"

otherChip(input1, input2){
  yourChipName(input1, input2)
}
```

This would run the yourChipName Chip.
