import time
import random

print("Generated from BNA code")
x = 5
y = random.randint(0, 14)
print(y)
x += y
z = x
y = random.randint(0, 11)
y += 1
z /= y
print(z)
z *= x
print(z)
time.sleep(2)
print(0)

