// Bütün "increase" və "decrease" düymələrini seçirik
const increaseButtons = document.querySelectorAll('.increase');
const decreaseButtons = document.querySelectorAll('.decrease');

// Artırma düyməsinə klik edildikdə işləyən funksionallıq
increaseButtons.forEach(button => {
  button.addEventListener('click', () => {
    const input = button.parentElement.querySelector('.quantity');
    input.value = parseInt(input.value) + 1;
  });
});

// Azaltma düyməsinə klik edildikdə işləyən funksionallıq
decreaseButtons.forEach(button => {
  button.addEventListener('click', () => {
    const input = button.parentElement.querySelector('.quantity');
    if (parseInt(input.value) > 1) {
      input.value = parseInt(input.value) - 1;
    }
  });
});