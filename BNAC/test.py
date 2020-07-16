import time
import random
import math
from goto import with_goto

@with_goto
def program():
	print("Generated from BNA code")
	count = input("Numbers to generate: ")
	try:
		count = float(count)
	except ValueError:
		count = count
	i = 0
	total = 0
	numbers = [0] * 0
	out_file = open("output.txt", 'a+')
	label .begin_loop
	success = 1 if i > count else 0
	if success != 0 :
		goto .end_loop
	x = random.randint(0, 9)
	x += 1
	print(x)
	numbers.append(x)
	out_file.write(str(numbers[i]))
	total += numbers[i]
	time.sleep(0.25)
	i += 1
	if 1 != 0 :
		goto .begin_loop
	label .end_loop
	out_file.close()
	average = total
	average /= count
	time.sleep(2)
	print("average = ")
	print(average)
	print("")
	in_file = open("input.txt", 'r')
	hello = in_file.readline()
	print(hello)
	in_file.close()
	print("")
	num = input("num = ")
	try:
		num = float(num)
	except ValueError:
		num = num
	val = num
	val -= 1
	print("\nnum - 1 = ")
	print(val)
	val = num
	val *= 3.5
	print("\nnum * 3.5 = ")
	print(val)
	val = num
	val = math.log(val, 10)
	print("\nlog_10( num ) = ")
	print(val)
	val = num
	val **= 2
	print("\nnum ** 2 = ")
	print(val)
	r_val = num
	r_val = int(round(r_val))
	print("\nnum rounded = r_num = ")
	print(r_val)
	val = r_val
	val %= 5
	print("\nr_num % 5 = ")
	print(val)
	val = r_val
	val = ~val
	print("\n!r_num = ")
	print(val)
	val = r_val
	val |= 7
	print("\nr_num | 7 = ")
	print(val)
	val = r_val
	val &= 7
	print("\nr_num & 7 = ")
	print(val)
	val = r_val
	val ^= 7
	print("\nr_num ^ 7 = ")
	print(val)

program()
