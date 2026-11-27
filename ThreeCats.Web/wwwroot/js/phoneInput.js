const handlers = new WeakMap();

function formatPhone(value) {
    let digits = value.replace(/\D/g, '');

    if (!digits.length) {
        return '';
    }

    if (digits[0] === '8') {
        digits = '7' + digits.slice(1);
    } else if (digits[0] !== '7') {
        digits = '7' + digits;
    }

    digits = digits.slice(0, 11);

    let result = '+7';

    if (digits.length > 1) {
        result += ' (' + digits.slice(1, 4);
    }

    if (digits.length >= 5) {
        result += ') ' + digits.slice(4, 7);
    }

    if (digits.length >= 8) {
        result += '-' + digits.slice(7, 9);
    }

    if (digits.length >= 10) {
        result += '-' + digits.slice(9, 11);
    }

    return result;
}

export function attach(element) {
    const beforeInputHandler = (event) => {
        if (!event.inputType.startsWith('insert')) {
            return;
        }

        if (event.inputType === 'insertFromPaste' ||
            event.inputType === 'insertFromDrop') {
            return;
        }

        if (event.data && !/^\d+$/.test(event.data)) {
            event.preventDefault();
        }
    };

    const inputHandler = () => {
        const originalValue = element.value;
        const selectionStart = element.selectionStart;
        const selectionEnd = element.selectionEnd;
        const selectionDirection = element.selectionDirection;

        const originalDigits = originalValue.replace(/\D/g, '');

        const addedCountryCode =
            originalDigits.length > 0 &&
            originalDigits[0] !== '7' &&
            originalDigits[0] !== '8';

        const digitsBeforeStart =
            countDigitsBefore(originalValue, selectionStart) +
            (addedCountryCode ? 1 : 0);

        const digitsBeforeEnd =
            countDigitsBefore(originalValue, selectionEnd) +
            (addedCountryCode ? 1 : 0);

        const formatted = formatPhone(originalValue);

        if (originalValue !== formatted) {
            element.value = formatted;

            const newSelectionStart = getPositionAfterDigits(
                formatted,
                digitsBeforeStart
            );

            const newSelectionEnd = getPositionAfterDigits(
                formatted,
                digitsBeforeEnd
            );

            element.setSelectionRange(
                newSelectionStart,
                newSelectionEnd,
                selectionDirection
            );
        }
    };

    element.addEventListener('beforeinput', beforeInputHandler);
    element.addEventListener('input', inputHandler);

    handlers.set(element, {
        beforeInputHandler,
        inputHandler
    });
}

export function detach(element) {
    const registered = handlers.get(element);

    if (!registered) {
        return;
    }

    element.removeEventListener(
        'beforeinput',
        registered.beforeInputHandler
    );

    element.removeEventListener(
        'input',
        registered.inputHandler
    );

    handlers.delete(element);
}

function countDigitsBefore(value, position) {
    return value.slice(0, position).replace(/\D/g, '').length;
}

function getPositionAfterDigits(value, digitCount) {
    if (digitCount === 0) {
        return 0;
    }

    let count = 0;

    for (let i = 0; i < value.length; i++) {
        if (/\d/.test(value[i])) {
            count++;

            if (count === digitCount) {
                return i + 1;
            }
        }
    }

    return value.length;
}
