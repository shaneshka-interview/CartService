<h1 class="code-line" data-line-start=0 data-line-end=1 ><a id="CartService_0"></a>CartService</h1>
<p class="has-line-data" data-line-start="2" data-line-end="11">Дано:<br>
Модель продукта<br>
class Product<br>
{<br>
public int Id { get; set; }<br>
public string Name { get; set; }<br>
public decimal Cost { get; set; }<br>
public bool ForBonusPoints { get; set; }<br>
}</p>
<p class="has-line-data" data-line-start="12" data-line-end="14">Задача:<br>
Реализовать сервис корзины CartService</p>
<p class="has-line-data" data-line-start="15" data-line-end="16">Требования:</p>
<ol>
<li class="has-line-data" data-line-start="16" data-line-end="18">
<p class="has-line-data" data-line-start="16" data-line-end="17">Стэк: Asp Net Core 3 WEB Api + хранилище на выбор Redis / Sql Server + Dapper</p>
</li>
<li class="has-line-data" data-line-start="18" data-line-end="19">
<p class="has-line-data" data-line-start="18" data-line-end="19">Функционал:</p>
</li>
</ol>
<ol>
<li class="has-line-data" data-line-start="19" data-line-end="20">Добавление / удаление произвольного числа продуктов</li>
<li class="has-line-data" data-line-start="20" data-line-end="21">Данные о корзине хранить в течение 30 дней</li>
<li class="has-line-data" data-line-start="21" data-line-end="22">Возможность регистрации веб хуков, которые нужно дергать при удалении корзины по истечению срока хранения</li>
<li class="has-line-data" data-line-start="22" data-line-end="28">1 раз в сутки генерировать и сохранять отчет (txt/pdf/excel на выбор), в котором будет указано:<br>
а) сколько всего корзин<br>
б) сколько из них содержат продукты за баллы<br>
в) сколько корзин истечет в течение 10/20/30 дней<br>
г) средний чек корзины</li>
</ol>
<h1 class="code-line" data-line-start=28 data-line-end=29 ><a id="_28"></a>Решение</h1>
<p class="has-line-data" data-line-start="30" data-line-end="31">В Settings настрока коннекта к БД (sqlite) и число дней активной корзины</p>
<p class="has-line-data" data-line-start="32" data-line-end="33">WebApi <a href="https://localhost:5001/api/carts/help">https://localhost:5001/api/carts/help</a></p>
<p class="has-line-data" data-line-start="34" data-line-end="35">ReportWorker - генерация отчета</p>
<p class="has-line-data" data-line-start="36" data-line-end="37">ExpireWorker - удаление корзин</p>
