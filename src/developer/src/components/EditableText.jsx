import { Button, ExternalLink } from '@ugrc/utah-design-system';
import PropTypes from 'prop-types';
import { useState } from 'react';
import TextArea from './design-system/TextArea';

const EditableText = ({ text, pattern, onChange }) => {
  const [editing, setEditing] = useState(false);
  const [note, setNote] = useState(text);
  const editable = pattern !== 'api-client.ugrc.utah.gov';

  if (!editable) {
    return (
      <div className="flex flex-col items-center gap-4 p-4 text-primary-800 dark:text-primary-200">
        <p>
          This API key is special and can only be used with the{' '}
          <ExternalLink href="https://gis.utah.gov/products/sgid/address/api-client/">
            UGRC API Client
          </ExternalLink>
          . It enables desktop geocoding of CSV files of addresses.
        </p>
      </div>
    );
  }
  return (
    <div className="flex flex-col items-center gap-4 p-4 text-primary-800 dark:text-primary-200">
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
            onPress={() => {
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
          <Button variant="destructive" onPress={() => setEditing(false)}>
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
