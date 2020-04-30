import time
import random
from goto import with_goto

@with_goto
def program():
	print("Generated from BNA code")
	I = 0
	COUNT = 7
	TOTAL = 0
	label .BEGIN
	SUCCESS = 1 if I > COUNT else 0
	if SUCCESS != 0 :
		goto .END
	X = random.randint(0, 9)
	X += 1
	print(X)
	TOTAL += X
	time.sleep(1)
	I += 1
	if 1 != 0 :
		goto .BEGIN
	label .END
	AVERAGE = TOTAL
	AVERAGE /= COUNT
	time.sleep(2)
	print(AVERAGE)

program()
