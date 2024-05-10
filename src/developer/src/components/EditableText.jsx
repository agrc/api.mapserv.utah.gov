import PropTypes from 'prop-types';
import { useState } from 'react';
import { TextLink } from './Link';
import Button from './design-system/Button';
import TextArea from './design-system/TextArea';

// eslint-disable-next-line no-unused-vars
const EditableText = ({ text, pattern, onChange }) => {
  const [editing, setEditing] = useState(false);
  const [note, setNote] = useState(text);
  const editable = pattern !== 'api-client.ugrc.utah.gov';

  if (!editable) {
    return (
      <div className="flex flex-col items-center gap-4 p-4 text-wavy-800 dark:text-wavy-200">
        <p>
          This API key is special and can only be used with the{' '}
          <TextLink href="https://gis.utah.gov/products/sgid/address/api-client/">
            UGRC API Client
          </TextLink>
          . It enables desktop geocoding of CSV files of addresses.
        </p>
      </div>
    );
  }
  return (
    <div className="flex flex-col items-center gap-4 p-4 text-wavy-800 dark:text-wavy-200">
      {!editing ? (
        note
      ) : (
        <TextArea
          className="w-full md:max-w-lg"
          label="Update note"
          value={note}
          onChange={setNote}
        />
      )}

      <div className="flex flex-row justify-between gap-2">
        {
          <Button
            appearance={Button.Appearances.solid}
            color={Button.Colors.primary}
            onClick={() => {
              setEditing(!editing);
              if (editing && note !== text) {
                onChange(note);
              }
            }}
          >
            {editing ? 'Save' : 'Edit'}
          </Button>
        }
        {editing && (
          <Button
            color={Button.Colors.secondary}
            onClick={() => setEditing(false)}
          >
            Cancel
          </Button>
        )}
      </div>
    </div>
  );
};

EditableText.propTypes = {
  text: PropTypes.string,
  pattern: PropTypes.string,
  onChange: PropTypes.func,
};

export default EditableText;
