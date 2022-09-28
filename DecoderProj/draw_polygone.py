#!/usr/bin/env python3
"""Writes bounding_box_and_polygon.png that illustrates
"""
from PIL import Image, ImageDraw
from pyzbar.pyzbar import decode
import argparse
import os
import numpy as np
import codecs, json 


# construct the argument parser and parse the arguments
ap = argparse.ArgumentParser()
ap.add_argument("-i", "--input", type=str, default="QR2.png",
	help="path for input image")
args = vars(ap.parse_args())

input_image_file = args["input"]
image = Image.open(input_image_file).convert('RGB')
draw = ImageDraw.Draw(image)
output_string = []
for barcode in decode(image):
"""
    rect = barcode.rect
    draw.rectangle(
        (
            (rect.left, rect.top),
            (rect.left + rect.width, rect.top + rect.height)
        ),
        outline='#0080ff'
    )
"""
    draw.polygon(barcode.polygon, outline='#e945ff')
    
    output_string = output_string + barcode.data.tolist() # nested lists with same data, indices

output_image_file = os.path.splitext(input_image_file)[0] + '_QR_box' + os.path.splitext(input_image_file)[1]


image.save(output_image_file)

output_string_file = os.path.splitext(input_image_file)[0] + '_string.json' 
json.dump(output_string, codecs.open(output_string_file, 'w', encoding='utf-8'), 
          separators=(',', ':'), 
          sort_keys=True, 
          indent=4) ### this saves the array in .json format

