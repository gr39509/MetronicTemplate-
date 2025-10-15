export function initializeSelectPicker(element) {
    $(element).selectpicker();
}

export function refreshSelectPicker(element) {
    $(element).selectpicker('refresh');
}

export function getSelectValue(element) {
    return $(element).val();
}

export function getSelectValues(element) {
    const values = $(element).val();
    return Array.isArray(values) ? values : [values];
}

export function setSelectValue(element, value) {
    $(element).selectpicker('val', value);
}

export function setSelectValues(element, values) {
    $(element).selectpicker('val', values);
}

export function destroySelectPicker(element) {
    $(element).selectpicker('destroy');
}