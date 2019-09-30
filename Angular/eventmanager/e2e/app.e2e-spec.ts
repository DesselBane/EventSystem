import { EventmanagerPage } from './app.po';

describe('eventmanager App', () => {
  let page: EventmanagerPage;

  beforeEach(() => {
    page = new EventmanagerPage();
  });

  it('should display message saying app works', () => {
    page.navigateTo();
    expect(page.getParagraphText()).toEqual('app works!');
  });
});
