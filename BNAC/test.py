import time
import random
from goto import with_goto

@with_goto
def program():
	print("Generated from BNA code")
	i = 0
	count = 7
	total = 0
	label .begin
	success = 1 if i > count else 0
	if success != 0 :
		goto .end
	x = random.randint(0, 9)
	x += 1
	print(x)
	total += x
	time.sleep(1)
	i += 1
	if 1 != 0 :
		goto .begin
	label .end
	average = total
	average /= count
	time.sleep(2)
	print(average)

program()
