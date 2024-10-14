function r_highlights(id) {
    let example_code = document.getElementById(id);
    let highlights_str = highlights(example_code.innerText);

    console.log(example_code.innerText);
    console.log(highlights_str);

    example_code.innerHTML = highlights_str;
}