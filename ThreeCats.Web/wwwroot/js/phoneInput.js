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
        const formatted = formatPhone(element.value);

        if (element.value !== formatted) {
            element.value = formatted;
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
