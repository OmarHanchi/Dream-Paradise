// Push Front
function pushFront(arr, val) {
    for (let i = arr.length; i > 0; i--) {
        arr[i] = arr[i - 1];
    }
    arr[0] = val;
    return arr;
}

console.log(pushFront([5, 7, 2, 3], 8)); // [8, 5, 7, 2, 3]
console.log(pushFront([99], 7)); // [7, 99]

// Pop Front
function popFront(arr) {
    const removedValue = arr[0];
    for (let i = 0; i < arr.length - 1; i++) {
        arr[i] = arr[i + 1];
    }
    arr.length--;
    console.log(arr);
    return removedValue;
}

console.log(popFront([0, 5, 10, 15])); // 0, [5, 10, 15]
console.log(popFront([4, 5, 7, 9])); // 4, [5, 7, 9]

// Insert At
function insertAt(arr, index, val) {
    for (let i = arr.length; i > index; i--) {
        arr[i] = arr[i - 1];
    }
    arr[index] = val;
    return arr;
}

console.log(insertAt([100, 200, 5], 2, 311)); // [100, 200, 311, 5]
console.log(insertAt([9, 33, 7], 1, 42)); // [9, 42, 33, 7]

// BONUS: Remove At
function removeAt(arr, index) {
    const removedValue = arr[index];
    for (let i = index; i < arr.length - 1; i++) {
        arr[i] = arr[i + 1];
    }
    arr.length--;
    console.log(arr);
    return removedValue;
}

console.log(removeAt([1000, 3, 204, 77], 1)); // 3, [1000, 204, 77]
console.log(removeAt([8, 20, 55, 44, 98], 3)); // 44, [8, 20, 55, 98]

// BONUS: Swap Pairs
function swapPairs(arr) {
    for (let i = 0; i < arr.length - 1; i += 2) {
        const temp = arr[i];
        arr[i] = arr[i + 1];
        arr[i + 1] = temp;
    }
    return arr;
}

console.log(swapPairs([1, 2, 3, 4])); // [2, 1, 4, 3]
console.log(swapPairs(["Brendan", true, 42])); // [true, "Brendan", 42]

// BONUS: Remove Duplicates
function removeDupes(arr) {
    if (arr.length === 0) return arr;
    let writeIndex = 1;
    for (let readIndex = 1; readIndex < arr.length; readIndex++) {
        if (arr[readIndex] !== arr[readIndex - 1]) {
            arr[writeIndex] = arr[readIndex];
            writeIndex++;
        }
    }
    arr.length = writeIndex;
    return arr;
}

console.log(removeDupes([-2, -2, 3.14, 5, 5, 10])); // [-2, 3.14, 5, 10]
console.log(removeDupes([9, 19, 19, 19, 19, 19, 29])); // [9, 19, 29]
