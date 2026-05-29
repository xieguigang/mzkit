import base64
import struct

def decode_mz_intensity(base64_str):
    # Decode the base64 string
  decoded_bytes = base64.b64decode(base64_str)

  # Convert the bytes to a double vector
  double_vector = []
  for i in range(0, len(decoded_bytes), 8):
      # Unpack 8 bytes to a double in network byte order (big endian)
      double_value = struct.unpack('>d', decoded_bytes[i:i+8])[0]
      double_vector.append(double_value)

  return double_vector

# Read the data from the file
with open("raw.txt", "r") as f:
  data = f.readlines()

for line in data:
  columns = line.strip().split("\t")
  # Extract the mz and intensity values
  mz_values = decode_mz_intensity(columns[3])
  intensity_values = decode_mz_intensity(columns[4])
  # Output the decoded values
  print("rt:", columns[0])
  print("bpc:", columns[1])
  print("tic:", columns[2])
  print("Decoded mz values:", mz_values)
  print("Decoded intensity values:", intensity_values)
  print("---------------------------")
